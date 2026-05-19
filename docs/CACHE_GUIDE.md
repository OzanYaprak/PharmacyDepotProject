# MediatR Pipeline Cache Yapısı — Uygulama Rehberi

Bu rehber, Clean Architecture + MediatR kullanan bir .NET projesinde **Distributed Cache (Redis)** entegrasyonunun adım adım nasıl yapıldığını, hangi sınıfların ne iş yaptığını ve yeni bir projede bu yapıyı sıfırdan nasıl uygulayacağını açıklar.

---

## İçindekiler

1. [Genel Mimari](#1-genel-mimari)
2. [Bileşenler ve Sorumlulukları](#2-bileşenler-ve-sorumlulukları)
3. [Adım Adım Uygulama](#3-adım-adım-uygulama)
4. [Yeni Bir Entity İçin Cache Nasıl Eklenir](#4-yeni-bir-entity-için-cache-nasıl-eklenir)
5. [Hata Ayıklama ve Sık Yapılan Hatalar](#5-hata-ayıklama-ve-sık-yapılan-hatalar)
6. [appsettings.json Yapılandırması](#6-appsettingsjson-yapılandırması)

---

## 1. Genel Mimari

```
HTTP İsteği
	│
	▼
[Controller]  →  ISender.Send(query/command)
	│
	▼
[MediatR Pipeline]
	├── RequestValidationBehavior   ← FluentValidation
	├── TransactionScopeBehavior    ← DB transaction
	├── CachingBehavior             ← Cache'ten oku veya yaz  (Query'ler için)
	└── CacheRemovingBehavior       ← Cache'i temizle         (Command'lar için)
	│
	▼
[Handler]  →  Repository  →  Veritabanı
```

**Temel kural:**
- **Query** (veri okuma) → `ICacheableRequest` implement eder → `CachingBehavior` devreye girer.
- **Command** (veri yazma/güncelleme/silme) → `ICacheRemoverRequest` implement eder → `CacheRemovingBehavior` devreye girer.

---

## 2. Bileşenler ve Sorumlulukları

### 2.1 `ICacheableRequest` (Okuma)

```csharp
// Application/Pipelines/Caching/Add/ICacheableRequest.cs
public interface ICacheableRequest
{
	string CacheKey { get; }          // Redis'teki benzersiz anahtar
	bool BypassCache { get; }         // true ise cache atlanır, direkt DB'ye gider
	string? CacheGroupKey { get; }    // Grup anahtarı (toplu silme için)
	TimeSpan? CacheExpiration { get; } // null ise appsettings'teki değer kullanılır
}
```

| Property | Açıklama |
|---|---|
| `CacheKey` | `$"{GetType().FullName}_{PageNumber}_{PageSize}"` gibi sayfa bazlı benzersiz key |
| `BypassCache` | `false` olarak sabitle; test/debug için `true` yapılabilir |
| `CacheGroupKey` | Aynı entity'nin tüm sayfa cache'lerini tek seferde temizlemek için (örn. `"GetCustomersQuery"`) |
| `CacheExpiration` | `null` bırakılırsa `appsettings.json → CacheSettings:ExpirationTime` (gün) kullanılır |

### 2.2 `ICacheRemoverRequest` (Yazma)

```csharp
// Application/Pipelines/Caching/Remove/ICacheRemoverRequest.cs
public interface ICacheRemoverRequest
{
	string? CacheKey { get; }          // Tekil bir key silinecekse doldur, yoksa null
	string? CacheGroupKey { get; }     // Tüm grup silinecekse doldur (önerilen)
	bool BypassCache { get; }          // true ise cache temizleme atlanır
}
```

> **İpucu:** Genellikle `CacheKey = null` ve `CacheGroupKey = "GetXxxQuery"` kombinasyonu kullanılır.
> Böylece Create/Update/Delete işlemlerinde tüm sayfa cache'leri tek seferde temizlenir.

### 2.3 `CachingBehavior<TRequest, TResponse>` (Pipeline — Okuma)

**Akış:**
1. `BypassCache == true` → direk handler'a geç.
2. Redis'te `CacheKey` var mı? → Varsa deserialize et, dön.
3. Yoksa → handler'ı çalıştır, sonucu Redis'e yaz.
4. `CacheGroupKey` doluysa → gruba bu key'i ekle (toplu silme için takip).

### 2.4 `CacheRemovingBehavior<TRequest, TResponse>` (Pipeline — Yazma)

**Akış:**
1. `BypassCache == true` → direk handler'a geç.
2. Handler'ı çalıştır (DB işlemi tamamlansın).
3. `CacheKey != null` → o tekil key'i sil.
4. `CacheGroupKey != null` → gruptaki **tüm** key'leri sil, ardından grup kaydını ve `_ExpirationTime` metadata'sını temizle.

### 2.5 `CacheSettings`

```csharp
// Application/Pipelines/Caching/CacheSettings.cs
public class CacheSettings
{
	public int ExpirationTime { get; set; } // Gün cinsinden varsayılan TTL
}
```

---

## 3. Adım Adım Uygulama

### Adım 1 — NuGet Paketleri

```xml
<!-- Application projesi -->
<PackageReference Include="Microsoft.Extensions.Caching.Abstractions" />

<!-- WebAPI / Persistence projesi -->
<PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" />
```

### Adım 2 — `CacheSettings` Sınıfı

```csharp
// Application/Pipelines/Caching/CacheSettings.cs
namespace Application.Pipelines.Caching;

public class CacheSettings
{
	public int ExpirationTime { get; set; } // Varsayılan: gün cinsinden
}
```

### Adım 3 — `ICacheableRequest` Arayüzü

```csharp
// Application/Pipelines/Caching/Add/ICacheableRequest.cs
namespace Application.Pipelines.Caching.Add;

public interface ICacheableRequest
{
	string CacheKey { get; }
	bool BypassCache { get; }
	string? CacheGroupKey { get; }
	TimeSpan? CacheExpiration { get; }
}
```

### Adım 4 — `ICacheRemoverRequest` Arayüzü

```csharp
// Application/Pipelines/Caching/Remove/ICacheRemoverRequest.cs
namespace Application.Pipelines.Caching.Remove;

public interface ICacheRemoverRequest
{
	string? CacheKey { get; }
	string? CacheGroupKey { get; }
	bool BypassCache { get; }
}
```

### Adım 5 — `CachingBehavior` (Okuma Pipeline'ı)

```csharp
// Application/Pipelines/Caching/Add/CachingBehavior.cs
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, ICacheableRequest
{
	// Constructor: IDistributedCache, IConfiguration, ILogger, CacheSettings inject edilir.
	// Handle:
	//   1. BypassCache kontrolü
	//   2. GetAsync(CacheKey) → cache hit ise deserialize et dön
	//   3. Cache miss ise handler çalıştır, SetAsync ile yaz
	//   4. CacheGroupKey varsa AddCacheKeyToGroup çağır
}
```

> ⚠️ **Kritik:** Grup kaydı var olan branch'te (`cachedGroupKeys != null`) yeni key eklendikten sonra
> mutlaka `SetAsync` çağrılmalıdır. Aksi halde grup kaydı güncellenmez ve toplu silme eksik çalışır.

### Adım 6 — `CacheRemovingBehavior` (Yazma Pipeline'ı)

```csharp
// Application/Pipelines/Caching/Remove/CacheRemovingBehavior.cs
public class CacheRemovingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, ICacheRemoverRequest
{
	// Handle:
	//   1. BypassCache kontrolü
	//   2. next() çağır (DB işlemi tamamlansın)
	//   3. CacheKey != null → RemoveAsync(CacheKey)
	//   4. CacheGroupKey != null → gruptaki tüm key'leri sil + grup meta'sını temizle
}
```

### Adım 7 — Pipeline Kaydı (DI)

```csharp
// Application/ApplicationServiceRegistration.cs
services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

	cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
	cfg.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
	cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
	cfg.AddOpenBehavior(typeof(CacheRemovingBehavior<,>)); // ← UNUTMA!
});
```

### Adım 8 — Program.cs Yapılandırması

```csharp
// WebAPI/Program.cs

// CacheSettings'i Singleton olarak kaydet
builder.Services.AddSingleton(
	builder.Configuration.GetSection("CacheSettings").Get<CacheSettings>()!
);

// Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = builder.Configuration["Redis:Configuration"];
	options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// Veya geliştirme ortamı için in-memory:
// builder.Services.AddDistributedMemoryCache();
```

---

## 4. Yeni Bir Entity İçin Cache Nasıl Eklenir

### 4.1 GetList Query — Cache'e Ekle

```csharp
public class GetListProductQuery : IRequest<GetListResponse<GetListProductListItemDto>>, ICacheableRequest
{
	public PageRequest? PageRequest { get; set; }

	// Sayfa bazlı benzersiz key — farklı sayfa/boyut farklı cache entry
	public string CacheKey => $"{GetType().FullName}_{PageRequest?.PageNumber}_{PageRequest?.PageSize}";

	// false: her zaman cache kullan
	public bool BypassCache => false;

	// Grup adı: Create/Update/Delete'te bu gruptaki tüm key'ler silinir
	public string? CacheGroupKey => "GetProductsQuery";

	// null: appsettings'teki varsayılan TTL kullanılır
	public TimeSpan? CacheExpiration => null;
}
```

### 4.2 Create/Update/Delete Command — Cache'i Temizle

```csharp
public class CreateProductCommand : IRequest<CreatedProductResponse>, ICacheRemoverRequest
{
	public string Name { get; set; } = default!;
	// ... diğer alanlar

	public string? CacheKey => null;                    // Tekil key silmiyoruz
	public string? CacheGroupKey => "GetProductsQuery"; // Tüm grubu sil
	public bool BypassCache => false;
}

public class UpdateProductCommand : IRequest<UpdatedProductResponse>, ICacheRemoverRequest
{
	public Guid Id { get; set; }
	// ... diğer alanlar

	public string? CacheKey => null;
	public string? CacheGroupKey => "GetProductsQuery";
	public bool BypassCache => false;
}

public class DeleteProductCommand : IRequest<DeletedProductResponse>, ICacheRemoverRequest
{
	public Guid Id { get; set; }

	public string? CacheKey => null;
	public string? CacheGroupKey => "GetProductsQuery";
	public bool BypassCache => false;
}
```

> **Kurallar:**
> - `CacheGroupKey` değeri Query'de ve Command'larda **aynı string** olmalıdır.
> - Sadece sayfalı listeler için değil, GetById Query'lerine de cache eklenebilir. O zaman `CacheGroupKey`'i aynı gruba dahil et.

---

## 5. Hata Ayıklama ve Sık Yapılan Hatalar

| Hata | Sebep | Çözüm |
|---|---|---|
| `NotImplementedException` fırlatılıyor | `ICacheRemoverRequest` implement edildi ama property'ler doldurulmadı | `CacheKey`, `CacheGroupKey`, `BypassCache` property'lerini gerçek değerlerle doldur |
| Command çalışıyor ama cache temizlenmiyor | `CacheRemovingBehavior` pipeline'a eklenmemiş | `ApplicationServiceRegistration`'da `cfg.AddOpenBehavior(typeof(CacheRemovingBehavior<,>))` ekle |
| Grup'a yeni key eklenmiyor | `AddCacheKeyToGroup`'ta `cachedGroupKeys != null` branch'inde `SetAsync` eksik | Grup güncellenince `await _distributedCache.SetAsync(...)` çağrıldığından emin ol |
| Cache hiç çalışmıyor | `IDistributedCache` servisi kayıtlı değil | `AddDistributedMemoryCache()` veya `AddStackExchangeRedisCache(...)` Program.cs'e ekle |
| Redis bağlantı hatası | `appsettings.json`'da Redis yapılandırması yanlış | Connection string formatını kontrol et: `"localhost:6379"` |
| `CacheSettings` null geldi | `AddSingleton(CacheSettings)` çağrılmamış | Program.cs'e `builder.Services.AddSingleton(...)` satırını ekle |

---

## 6. appsettings.json Yapılandırması

```json
{
  "CacheSettings": {
	"ExpirationTime": 1
  },
  "Redis": {
	"Configuration": "localhost:6379",
	"InstanceName": "PharmacyDepot_"
  }
}
```

| Alan | Açıklama |
|---|---|
| `CacheSettings:ExpirationTime` | Varsayılan cache süresi — **gün** cinsinden. `ICacheableRequest.CacheExpiration` null ise bu kullanılır |
| `Redis:Configuration` | Redis sunucusu bağlantı string'i. Production'da `"redis-host:6379,password=xxx"` formatı |
| `Redis:InstanceName` | Tüm key'lerin önüne eklenen prefix. Farklı uygulamalar aynı Redis'i paylaşıyorsa çakışmayı önler |

---

## Özet — Kontrol Listesi

Yeni projede cache yapısını kurarken şu adımları sırayla takip et:

- [ ] `CacheSettings.cs` sınıfını oluştur
- [ ] `ICacheableRequest.cs` arayüzünü oluştur
- [ ] `ICacheRemoverRequest.cs` arayüzünü oluştur
- [ ] `CachingBehavior.cs` pipeline behavior'ını oluştur
- [ ] `CacheRemovingBehavior.cs` pipeline behavior'ını oluştur
- [ ] `ApplicationServiceRegistration.cs`'de her iki behavior'ı pipeline'a ekle
- [ ] `Program.cs`'de `AddSingleton<CacheSettings>()` ve `AddStackExchangeRedisCache()` ekle
- [ ] `appsettings.json`'a `CacheSettings` ve `Redis` bölümlerini ekle
- [ ] Her GetList Query'sine `ICacheableRequest` implement et
- [ ] Her Create/Update/Delete Command'ına `ICacheRemoverRequest` implement et
- [ ] `CacheGroupKey` değerinin Query ve Command'larda **tutarlı** olduğunu doğrula
