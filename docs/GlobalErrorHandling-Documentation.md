# PharmacyDepot — Global Hata Yönetimi Dokümantasyonu

> **Hazırlayan:** GitHub Copilot  
> **Proje:** PharmacyDepot (.NET 10, Clean Architecture)  
> **Konu:** `CrossCuttingConcerns` katmanındaki global exception yönetiminin tam açıklaması, yapılan iyileştirmeler ve kullanım rehberi

---

## İçindekiler

- [PharmacyDepot — Global Hata Yönetimi Dokümantasyonu](#pharmacydepot--global-hata-yönetimi-dokümantasyonu)
  - [İçindekiler](#i̇çindekiler)
  - [1. Neden Global Hata Yönetimi?](#1-neden-global-hata-yönetimi)
    - [Problem: Her Yerde Try/Catch](#problem-her-yerde-trycatch)
    - [Çözüm: Merkezi Middleware](#çözüm-merkezi-middleware)
  - [2. Mimari Genel Bakış](#2-mimari-genel-bakış)
    - [Katmanlar Arasındaki İlişki](#katmanlar-arasındaki-i̇lişki)
  - [3. Dosya Yapısı](#3-dosya-yapısı)
  - [4. Bileşenler — Detaylı Açıklamalar](#4-bileşenler--detaylı-açıklamalar)
    - [4.1 Exception Türleri (Types)](#41-exception-türleri-types)
      - [`BusinessException`](#businessexception)
      - [`NotFoundException` _(YENİ)_](#notfoundexception-yeni̇)
    - [4.2 Problem Details (HTTP Yanıt Modelleri)](#42-problem-details-http-yanıt-modelleri)
      - [`BusinessProblemDetails`](#businessproblemdetails)
      - [`NotFoundProblemDetails` _(YENİ)_](#notfoundproblemdetails-yeni̇)
      - [`InternalServerErrorProblemDetails`](#internalservererrorproblemdetails)
    - [4.3 ExceptionHandler (Soyut Temel Sınıf)](#43-exceptionhandler-soyut-temel-sınıf)
    - [4.4 HttpExceptionHandler](#44-httpexceptionhandler)
    - [4.5 ExceptionMiddleware](#45-exceptionmiddleware)
    - [4.6 Extension Metodlar](#46-extension-metodlar)
      - [`ExceptionMiddlewareExtensions`](#exceptionmiddlewareextensions)
      - [`ProblemDetailsExtensions`](#problemdetailsextensions)
  - [5. İstek Akışı — Adım Adım](#5-i̇stek-akışı--adım-adım)
    - [Senaryo A: İş Kuralı İhlali (400)](#senaryo-a-i̇ş-kuralı-i̇hlali-400)
    - [Senaryo B: Kayıt Bulunamadı (404)](#senaryo-b-kayıt-bulunamadı-404)
    - [Senaryo C: Beklenmeyen Hata (500)](#senaryo-c-beklenmeyen-hata-500)
  - [6. HTTP Durum Kodları Eşlemesi](#6-http-durum-kodları-eşlemesi)
  - [7. RFC 7807 Problem Details Standardı](#7-rfc-7807-problem-details-standardı)
    - [Standart JSON Yanıt Formatı](#standart-json-yanıt-formatı)
    - [Her Alan Ne İfade Eder?](#her-alan-ne-i̇fade-eder)
    - [Neden Bu Standart?](#neden-bu-standart)
  - [8. Yapılan İyileştirmeler](#8-yapılan-i̇yileştirmeler)
    - [8.1 Kritik Bug Düzeltmesi](#81-kritik-bug-düzeltmesi)
    - [8.2 NotFoundException Desteği](#82-notfoundexception-desteği)
  - [9. Kullanım Rehberi — Yeni Kural Nasıl Yazılır?](#9-kullanım-rehberi--yeni-kural-nasıl-yazılır)
    - [Yeni Bir İş Kuralı Eklemek (BusinessException)](#yeni-bir-i̇ş-kuralı-eklemek-businessexception)
    - [Yeni Bir Entity için NotFoundException Eklemek](#yeni-bir-entity-için-notfoundexception-eklemek)
  - [10. Örnek API Yanıtları](#10-örnek-api-yanıtları)
    - [400 Bad Request — İş Kuralı İhlali](#400-bad-request--i̇ş-kuralı-i̇hlali)
    - [404 Not Found — Kayıt Bulunamadı](#404-not-found--kayıt-bulunamadı)
    - [500 Internal Server Error — Beklenmeyen Hata](#500-internal-server-error--beklenmeyen-hata)
  - [11. Program.cs — Middleware Sırası ve Önemi](#11-programcs--middleware-sırası-ve-önemi)
    - [Neden En Başta?](#neden-en-başta)
  - [12. İleride Yapılabilecek Geliştirmeler](#12-i̇leride-yapılabilecek-geliştirmeler)
    - [1. Production'da Hata Mesajı Gizleme](#1-productionda-hata-mesajı-gizleme)
    - [2. Loglama (Logging) Entegrasyonu](#2-loglama-logging-entegrasyonu)
    - [3. ValidationException Desteği (FluentValidation)](#3-validationexception-desteği-fluentvalidation)
    - [4. `instance` Alanı ile İstek Takibi](#4-instance-alanı-ile-i̇stek-takibi)
    - [5. Tüm Entity'lere `DrugMustExistWhenRequested` Benzeri Kural Eklenmesi](#5-tüm-entitylere-drugmustexistwhenrequested-benzeri-kural-eklenmesi)

---

## 1. Neden Global Hata Yönetimi?

### Problem: Her Yerde Try/Catch

Global hata yönetimi olmadan, her controller metodu kendi hatasını elle yönetmek zorunda kalır:

```csharp
// ❌ KÖTÜ — Her endpoint'te tekrar eden kod
[HttpGet("{id}")]
public async Task<IActionResult> GetDrugById(Guid id)
{
    try
    {
        var drug = await _drugRepository.GetAsync(d => d.Id == id);
        if (drug == null)
            return NotFound("İlaç bulunamadı.");
        return Ok(drug);
    }
    catch (BusinessException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}
```

Bu yaklaşımın sorunları:

- **Tekrar (Duplication):** 50 endpoint varsa 50 kez aynı try/catch blokları.
- **Tutarsızlık:** Farklı geliştiriciler farklı hata formatları döndürür.
- **Bakım zorluğu:** Hata formatı değiştiğinde 50 dosya güncellenmeli.
- **SRP İhlali:** Controller hem iş mantığını hem hata yönetimini yapıyor.

### Çözüm: Merkezi Middleware

```csharp
// ✅ İYİ — Tüm exception'lar tek noktada yakalanır
[HttpGet("{id}")]
public async Task<IActionResult> GetDrugById(Guid id)
{
    var drug = await _drugRepository.GetAsync(d => d.Id == id);
    return Ok(drug);
    // Exception fırlarsa ExceptionMiddleware yakalar, controller bilmez bile
}
```

---

## 2. Mimari Genel Bakış

```
┌─────────────────────────────────────────────────────────┐
│                    HTTP İsteği (Client)                  │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│              ExceptionMiddleware (Pipeline)              │
│  ┌──────────────────────────────────────────────────┐   │
│  │  try { await _next(httpContext); }               │   │
│  │  catch (Exception ex) { HandleExceptionAsync }   │   │
│  └──────────────────────────────────────────────────┘   │
└───────────────────────┬─────────────────────────────────┘
                        │ exception fırlarsa
                        ▼
┌─────────────────────────────────────────────────────────┐
│              HttpExceptionHandler                        │
│                                                          │
│  BusinessException  → 400 + BusinessProblemDetails      │
│  NotFoundException  → 404 + NotFoundProblemDetails       │
│  Exception (diğer)  → 500 + InternalServerErrorDetails  │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│              RFC 7807 JSON Yanıtı (Client'a)             │
│  { "title": "...", "detail": "...", "status": ... }      │
└─────────────────────────────────────────────────────────┘
```

### Katmanlar Arasındaki İlişki

```
CrossCuttingConcerns (bu katman hiçbir uygulama katmanına bağlı değildir)
       ↑
   WebAPI  ←  Application  ←  Domain
       ↑
  Persistence / Infrastructure
```

`CrossCuttingConcerns` kasıtlı olarak bağımsız bir katmandır. Tüm katmanlar ona bağlanabilir ama o hiçbirine bağlanmaz.

---

## 3. Dosya Yapısı

```
CrossCuttingConcerns/
└── Exceptions/
    ├── Types/                          ← Exception sınıfları (ne fırlatılacak)
    │   ├── BusinessException.cs        ← İş kuralı ihlali (400)
    │   └── NotFoundException.cs        ← Kayıt bulunamadı (404)
    │
    ├── HttpProblemDetails/             ← HTTP yanıt modelleri (nasıl döneceği)
    │   ├── BusinessProblemDetails.cs   ← 400 için RFC 7807 modeli
    │   ├── NotFoundProblemDetails.cs   ← 404 için RFC 7807 modeli
    │   └── InternalServerErrorProblemDetails.cs  ← 500 için RFC 7807 modeli
    │
    ├── Handlers/                       ← Exception → HTTP yanıt dönüşümü
    │   ├── ExceptionHandler.cs         ← Soyut temel (Template Method deseni)
    │   └── HttpExceptionHandler.cs     ← HTTP'ye özgü implementasyon
    │
    ├── Middlewares/
    │   └── ExceptionMiddleware.cs      ← Pipeline'a giren asıl yakalayıcı
    │
    └── Extensions/
        ├── ExceptionMiddlewareExtensions.cs  ← app.UseCustomExceptionMiddleware()
        └── ProblemDetailsExtensions.cs       ← .AsJson() extension metodu
```

---

## 4. Bileşenler — Detaylı Açıklamalar

### 4.1 Exception Türleri (Types)

#### `BusinessException`

```csharp
public class BusinessException : Exception
{
    public BusinessException(string? message) : base(message) { }
}
```

**Ne zaman fırlatılır?**

- Kullanıcı hatalı/geçersiz veri gönderdiğinde
- İş kuralı ihlal edildiğinde (aynı GTIN, geçmiş tarih, vb.)
- Kullanıcının düzeltebileceği bir hata olduğunda

**Örnek kullanımlar:**

```csharp
throw new BusinessException(DrugMessages.GtinExists);
// → "A drug with this GTIN already exists."

throw new BusinessException(CustomerMessages.LicenseNumberExists);
// → "A customer with this license number already exists."
```

**HTTP Sonucu:** `400 Bad Request`

---

#### `NotFoundException` _(YENİ)_

```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' not found.") { }
}
```

**Ne zaman fırlatılır?**

- Veritabanında aranılan kayıt bulunamadığında
- GetById, Update veya Delete işlemlerinde kayıt yoksa

**Örnek kullanımlar:**

```csharp
throw new NotFoundException(nameof(Drug), id);
// → "Drug with id '3fa85f64-5717-4562-b3fc-2c963f66afa6' not found."

throw new NotFoundException(nameof(Customer), customerId);
// → "Customer with id '...' not found."
```

**HTTP Sonucu:** `404 Not Found`

---

### 4.2 Problem Details (HTTP Yanıt Modelleri)

Tüm modeller `Microsoft.AspNetCore.Mvc.ProblemDetails` sınıfından türer. Bu sınıf **RFC 7807** standardını implement eder ve şu alanları içerir:

| Alan     | Tür    | Açıklama                         |
| -------- | ------ | -------------------------------- |
| `title`  | string | Hata kategorisinin kısa adı      |
| `detail` | string | Hatayla ilgili spesifik açıklama |
| `status` | int    | HTTP durum kodu                  |
| `type`   | string | Hata türünü belgeleyen URI       |

#### `BusinessProblemDetails`

```csharp
Title  = "Rule Violation"
Status = 400
Type   = "https://example.com/probs/business-rule-violation"
```

#### `NotFoundProblemDetails` _(YENİ)_

```csharp
Title  = "Not Found"
Status = 404
Type   = "https://example.com/probs/not-found"
```

#### `InternalServerErrorProblemDetails`

```csharp
Title  = "Internal Server Error"
Status = 500
Type   = "https://example.com/probs/internal-server-error"
```

> ⚠️ **Production Uyarısı:** `InternalServerErrorProblemDetails` şu an exception mesajını `detail` alanına yazar. Production ortamında bu alan stack trace veya hassas bilgi içerebilir. Çözüm için bkz. [Bölüm 12](#12-i̇leride-yapılabilecek-geliştirmeler).

---

### 4.3 ExceptionHandler (Soyut Temel Sınıf)

**Tasarım Deseni:** Template Method Pattern

```csharp
public abstract class ExceptionHandler
{
    // Şablon metot: hangi tür exception → hangi handler kararını verir
    public Task HandleExceptionAsync(Exception exception) =>
        exception switch
        {
            BusinessException e  => HandleException(e),   // 400
            NotFoundException e  => HandleException(e),   // 404
            _                    => HandleException(exception) // 500
        };

    // Alt sınıfların implement etmesi ZORUNLU üç metot:
    protected abstract Task HandleException(BusinessException e);
    protected abstract Task HandleException(NotFoundException e);
    protected abstract Task HandleException(Exception e);
}
```

**Neden Abstract?**
Gelecekte farklı ortamlar için farklı handler'lar eklenebilir:

- `HttpExceptionHandler` → HTTP yanıtı yazar (şu an kullanılan)
- `ConsoleExceptionHandler` → Console uygulaması için (ileride eklenebilir)
- `GrpcExceptionHandler` → gRPC için (ileride eklenebilir)

---

### 4.4 HttpExceptionHandler

`ExceptionHandler`'dan türer ve exception'ları HTTP yanıtına dönüştürür:

```
BusinessException  ──→  StatusCode 400  +  BusinessProblemDetails JSON
NotFoundException  ──→  StatusCode 404  +  NotFoundProblemDetails JSON
Exception (diğer)  ──→  StatusCode 500  +  InternalServerErrorProblemDetails JSON
```

**Önemli Detay — `Response` Property:**

```csharp
public HttpResponse Response
{
    get => _httpResponse ?? throw new ArgumentNullException(nameof(_httpResponse));
    set => _httpResponse = value;
}
```

`HttpExceptionHandler` tek bir instance olarak yaşar (Middleware constructor'ında oluşturulur). Her istek için `Response` property'si güncellenir. `get` içindeki null kontrolü, Response set edilmeden kullanılmaya çalışıldığında anlamlı bir hata verir.

---

### 4.5 ExceptionMiddleware

```csharp
public async Task Invoke(HttpContext httpContext)
{
    try
    {
        await _next(httpContext);   // Sonraki middleware/controller'ı çağır
    }
    catch (Exception exception)
    {
        // Herhangi bir yerden fırlayan exception buraya düşer
        await HandleExceptionAsync(httpContext.Response, exception);
    }
}
```

**Neden `Invoke` metodu?**
ASP.NET Core middleware'leri ya `Invoke(HttpContext)` ya da `InvokeAsync(HttpContext)` metodunu implement eder. Framework her HTTP isteğinde bu metodu çağırır.

**`_next` nedir?**
`RequestDelegate` tipinde bir delege. Pipeline'daki bir sonraki middleware'i temsil eder. `await _next(httpContext)` çağrılmazsa istek ilerlemez (cevap dönmez).

---

### 4.6 Extension Metodlar

#### `ExceptionMiddlewareExtensions`

```csharp
public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
{
    app.UseMiddleware<ExceptionMiddleware>();
}
```

Kullanımı (`Program.cs`):

```csharp
app.UseCustomExceptionMiddleware();
```

Bu pattern'in faydası: `Program.cs` temiz kalır, middleware kaydı detayları `CrossCuttingConcerns` katmanında kapsüllenir.

---

#### `ProblemDetailsExtensions`

```csharp
public static string AsJson<TProblemDetail>(this TProblemDetail problemDetails)
    where TProblemDetail : ProblemDetails
{
    return JsonSerializer.Serialize(problemDetails);
}
```

**Generic Kısıt (`where TProblemDetail : ProblemDetails`):**
Yalnızca `ProblemDetails` türevleri için çalışır. Derleme zamanında tip güvenliği sağlar; yanlış türde nesne geçirilirse kod derlenmez.

**Kullanım:**

```csharp
string json = new BusinessProblemDetails("Hata mesajı").AsJson();
// → {"title":"Rule Violation","detail":"Hata mesajı","status":400,...}
```

---

## 5. İstek Akışı — Adım Adım

### Senaryo A: İş Kuralı İhlali (400)

```
1. Client → POST /api/drugs  (aynı GTIN ile)
2. ExceptionMiddleware.Invoke() → try { await _next(...) }
3. DrugsController.CreateDrug() → Mediator.Send(command)
4. CreateDrugCommandHandler.Handle()
5. DrugBusinessRules.GtinCannotBeDuplicatedWhenInserted()
6.   → throw new BusinessException("A drug with this GTIN already exists.")
7. Exception stack'i geri sarılır
8. ExceptionMiddleware catch bloğu devreye girer
9. HandleExceptionAsync(response, exception) çağrılır
10. HttpExceptionHandler.HandleExceptionAsync() → switch: BusinessException
11. HttpExceptionHandler.HandleException(BusinessException)
12.   → Response.StatusCode = 400
13.   → new BusinessProblemDetails("A drug...").AsJson() yazılır
14. Client ← HTTP 400 + JSON
```

### Senaryo B: Kayıt Bulunamadı (404)

```
1. Client → GET /api/drugs/{id}  (var olmayan ID)
2. ExceptionMiddleware.Invoke() → try { await _next(...) }
3. DrugsController.GetDrugById() → Mediator.Send(query)
4. GetByIdDrugQueryHandler.Handle()
5. DrugBusinessRules.DrugMustExistWhenRequested(id)
6.   → throw new NotFoundException("Drug", id)
7. Exception stack'i geri sarılır
8. ExceptionMiddleware catch bloğu devreye girer
9. HttpExceptionHandler → switch: NotFoundException
10.  → Response.StatusCode = 404
11.  → new NotFoundProblemDetails("Drug with id '...' not found.").AsJson()
12. Client ← HTTP 404 + JSON
```

### Senaryo C: Beklenmeyen Hata (500)

```
1. Client → GET /api/drugs
2. ExceptionMiddleware.Invoke() → try { await _next(...) }
3. Herhangi bir yerde NullReferenceException fırlar
4. ExceptionMiddleware catch bloğu devreye girer
5. HttpExceptionHandler → switch: _ (varsayılan dal)
6.   → Response.StatusCode = 500
7.   → new InternalServerErrorProblemDetails(ex.Message).AsJson()
8. Client ← HTTP 500 + JSON
```

---

## 6. HTTP Durum Kodları Eşlemesi

| Exception Türü      | HTTP Kodu                   | Açıklama                       | Kullanım Yeri              |
| ------------------- | --------------------------- | ------------------------------ | -------------------------- |
| `BusinessException` | `400 Bad Request`           | Kullanıcı hatalı veri gönderdi | `*BusinessRules` sınıfları |
| `NotFoundException` | `404 Not Found`             | İstenen kayıt bulunamadı       | `*BusinessRules` sınıfları |
| `Exception` (diğer) | `500 Internal Server Error` | Sunucu hatası                  | Otomatik yakalanır         |

---

## 7. RFC 7807 Problem Details Standardı

[RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807), HTTP API'lerinin hata yanıtlarını standart bir JSON formatında döndürmesini tanımlar. PharmacyDepot bu standardı tamamen uygular.

### Standart JSON Yanıt Formatı

```json
{
  "type": "https://example.com/probs/business-rule-violation",
  "title": "Rule Violation",
  "status": 400,
  "detail": "A drug with this GTIN already exists."
}
```

### Her Alan Ne İfade Eder?

| Alan       | Zorunlu   | Açıklama                                                                 |
| ---------- | --------- | ------------------------------------------------------------------------ |
| `type`     | Önerilen  | Hata kategorisini belgeleyen URI. Geliştirici belgelerine link olabilir. |
| `title`    | Önerilen  | Hata kategorisinin kısa, insan tarafından okunabilir özeti.              |
| `status`   | Önerilen  | HTTP durum kodu (integer).                                               |
| `detail`   | Opsiyonel | Bu spesifik hataya ilişkin açıklama.                                     |
| `instance` | Opsiyonel | Bu spesifik hata olayını tanımlayan URI (örn. istek ID'si).              |

### Neden Bu Standart?

- **İstemci uyumluluğu:** Frontend, mobil veya üçüncü taraf uygulamalar standart formatı parse etmeyi bilir.
- **Araç desteği:** Postman, Swagger UI gibi araçlar `ProblemDetails` formatını görsel olarak işler.
- **Tutarlılık:** Tüm API hataları aynı yapıda döner; istemci farklı format beklentisi taşımaz.

---

## 8. Yapılan İyileştirmeler

### 8.1 Kritik Bug Düzeltmesi

**Sorun:** `HttpExceptionHandler` içinde `HandleException(Exception exception)` metodu — yani 500 hataları için olan metot — yanlışlıkla `BusinessProblemDetails` kullanıyordu:

```csharp
// ❌ ESKİ — YANLIŞ: 500 hatası için 400'ün modeli kullanılıyordu
protected override Task HandleException(Exception exception)
{
    Response.StatusCode = StatusCodes.Status500InternalServerError;
    string details = new BusinessProblemDetails(exception.Message).AsJson(); // HATA!
    return Response.WriteAsync(details);
}
```

**Sonuç:** HTTP kodu 500 dönüyordu ama JSON body içinde `"title": "Rule Violation"` ve `"status": 400` yazıyordu. İstemci yanıltıcı bilgi alıyordu.

```csharp
// ✅ YENİ — DOĞRU
protected override Task HandleException(Exception exception)
{
    Response.StatusCode = StatusCodes.Status500InternalServerError;
    string details = new InternalServerErrorProblemDetails(exception.Message).AsJson();
    return Response.WriteAsync(details);
}
```

---

### 8.2 NotFoundException Desteği

**Önceki durum:** Kayıt bulunamadığında exception fırlatılmıyordu — handler'lar `null` dönüyor, AutoMapper null entity'yi boş response'a map'liyordu. İstemci 200 OK + boş/null nesne alıyordu.

**Eklenenler:**

| Dosya                                          | Değişiklik                                                |
| ---------------------------------------------- | --------------------------------------------------------- |
| `Types/NotFoundException.cs`                   | Yeni exception türü                                       |
| `HttpProblemDetails/NotFoundProblemDetails.cs` | Yeni HTTP yanıt modeli                                    |
| `Handlers/ExceptionHandler.cs`                 | `HandleException(NotFoundException)` soyut metodu eklendi |
| `Handlers/HttpExceptionHandler.cs`             | 404 → `NotFoundProblemDetails` implementasyonu eklendi    |
| `Features/Drugs/Rules/DrugBusinessRules.cs`    | `DrugMustExistWhenRequested(Guid id)` metodu eklendi      |

**Kullanım örneği:**

```csharp
// DrugBusinessRules içinde
public async Task DrugMustExistWhenRequested(Guid id)
{
    Drug? drug = await _drugRepository.GetAsync(predicate: d => d.Id == id);

    if (drug is null)
        throw new NotFoundException(nameof(Drug), id);
}
```

```csharp
// GetByIdDrugQueryHandler içinde (örnek kullanım)
public async Task<GetByIdDrugResponse> Handle(GetByIdDrugQuery request, CancellationToken cancellationToken)
{
    await _drugBusinessRules.DrugMustExistWhenRequested(request.Id); // önce kontrol
    Drug drug = await _drugRepository.GetAsync(d => d.Id == request.Id);
    return _mapper.Map<GetByIdDrugResponse>(drug);
}
```

---

## 9. Kullanım Rehberi — Yeni Kural Nasıl Yazılır?

### Yeni Bir İş Kuralı Eklemek (BusinessException)

**1. Mesaj sabitini tanımla (`*Messages.cs`):**

```csharp
// Application/Features/Warehouses/Constants/WarehouseMessages.cs
public class WarehouseMessages
{
    public const string CapacityExceeded = "Warehouse capacity has been exceeded.";
}
```

**2. BusinessRules sınıfına metod ekle:**

```csharp
// Application/Features/Warehouses/Rules/WarehouseBusinessRules.cs
public async Task CapacityCannotBeExceeded(Guid warehouseId, int requestedQuantity)
{
    Warehouse? warehouse = await _warehouseRepository.GetAsync(w => w.Id == warehouseId);

    if (warehouse != null && warehouse.CurrentStock + requestedQuantity > warehouse.Capacity)
        throw new BusinessException(WarehouseMessages.CapacityExceeded);
}
```

**3. Handler'da kuralı çağır:**

```csharp
public async Task<CreatedStockResponse> Handle(CreateStockCommand request, CancellationToken ct)
{
    await _warehouseBusinessRules.CapacityCannotBeExceeded(request.WarehouseId, request.Quantity);
    // devam...
}
```

---

### Yeni Bir Entity için NotFoundException Eklemek

**1. Mesaj sabitini tanımla:**

```csharp
public class WarehouseMessages
{
    public const string NotFound = "Warehouse not found.";
}
```

**2. BusinessRules sınıfına ekle:**

```csharp
public async Task WarehouseMustExistWhenRequested(Guid id)
{
    Warehouse? warehouse = await _warehouseRepository.GetAsync(w => w.Id == id);

    if (warehouse is null)
        throw new NotFoundException(nameof(Warehouse), id);
}
```

**3. İlgili Query/Command handler'larında kullan:**

```csharp
// GetByIdWarehouseQueryHandler, UpdateWarehouseCommandHandler, DeleteWarehouseCommandHandler
await _warehouseBusinessRules.WarehouseMustExistWhenRequested(request.Id);
```

---

## 10. Örnek API Yanıtları

### 400 Bad Request — İş Kuralı İhlali

```http
POST /api/drugs
Content-Type: application/json

{
  "name": "Aspirin",
  "gtin": "08681234567890",  ← zaten var
  ...
}
```

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "type": "https://example.com/probs/business-rule-violation",
  "title": "Rule Violation",
  "status": 400,
  "detail": "A drug with this GTIN already exists."
}
```

---

### 404 Not Found — Kayıt Bulunamadı

```http
GET /api/drugs/00000000-0000-0000-0000-000000000099
```

```http
HTTP/1.1 404 Not Found
Content-Type: application/json

{
  "type": "https://example.com/probs/not-found",
  "title": "Not Found",
  "status": 404,
  "detail": "Drug with id '00000000-0000-0000-0000-000000000099' not found."
}
```

---

### 500 Internal Server Error — Beklenmeyen Hata

```http
GET /api/drugs
```

```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{
  "type": "https://example.com/probs/internal-server-error",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Object reference not set to an instance of an object."
}
```

---

## 11. Program.cs — Middleware Sırası ve Önemi

```csharp
var app = builder.Build();

// 1. Exception Middleware — EN BAŞTA olmalı
app.UseCustomExceptionMiddleware();

// 2. HTTPS Yönlendirme
app.UseHttpsRedirection();

// 3. Kimlik Doğrulama / Yetkilendirme
app.UseAuthorization();

// 4. Controller Route'ları
app.MapControllers();
```

### Neden En Başta?

ASP.NET Core middleware'leri **sıralı bir zincir** oluşturur. İstek yukarıdan aşağıya, yanıt aşağıdan yukarıya akar:

```
İstek →  [ExceptionMiddleware]
              → [HttpsRedirection]
                  → [Authorization]
                      → [Controller]
                      ← (yanıt veya exception)
                  ← (exception yukarı taşınır)
              ← (exception yukarı taşınır)
         [ExceptionMiddleware] YAKALAR ✓
```

Eğer `UseCustomExceptionMiddleware()` en başta eklenmezse, ondan önce gelen middleware'lerden fırlayan exception'lar yakalanmaz.

---

## 12. İleride Yapılabilecek Geliştirmeler

### 1. Production'da Hata Mesajı Gizleme

Şu an 500 hatalarında exception mesajı istemciye iletilmektedir. Bu production ortamında güvenlik riski oluşturabilir.

```csharp
// HttpExceptionHandler içinde önerilen iyileştirme
protected override Task HandleException(Exception exception)
{
    Response.StatusCode = StatusCodes.Status500InternalServerError;

    // Production'da detayı gizle
    var isProduction = /* IWebHostEnvironment inject edilmeli */;
    var detail = isProduction
        ? "An unexpected error occurred. Please try again later."
        : exception.Message;

    string details = new InternalServerErrorProblemDetails(detail).AsJson();
    return Response.WriteAsync(details);
}
```

### 2. Loglama (Logging) Entegrasyonu

Şu an exception'lar yakalanıyor ama loglanmıyor. `ILogger` enjekte edilerek her hata loglanabilir:

```csharp
public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _exceptionHandler = new HttpExceptionHandler();
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception for {Path}", httpContext.Request.Path);
            await HandleExceptionAsync(httpContext.Response, exception);
        }
    }
}
```

### 3. ValidationException Desteği (FluentValidation)

FluentValidation entegre edildiğinde validasyon hatalarının da merkezi olarak yönetilmesi gerekir:

```csharp
// Yeni exception türü
public class ValidationException : Exception
{
    public IEnumerable<ValidationFailure> Errors { get; }
    public ValidationException(IEnumerable<ValidationFailure> errors) { ... }
}

// Yeni ProblemDetails
public class ValidationProblemDetails : ProblemDetails { ... }  // Status = 422
```

### 4. `instance` Alanı ile İstek Takibi

RFC 7807'nin `instance` alanı kullanılarak her hata yanıtına benzersiz istek ID'si eklenebilir. Bu sayede log sisteminde hata kolayca bulunur:

```json
{
  "title": "Rule Violation",
  "status": 400,
  "detail": "...",
  "instance": "/api/drugs/request-id-abc123"
}
```

### 5. Tüm Entity'lere `DrugMustExistWhenRequested` Benzeri Kural Eklenmesi

Şu an yalnızca `DrugBusinessRules` içinde bu kontrol mevcut. Diğer entity'ler için de eklenmesi gerekir:

- `CustomerBusinessRules.CustomerMustExistWhenRequested`
- `SupplierBusinessRules.SupplierMustExistWhenRequested`
- `WarehouseBusinessRules.WarehouseMustExistWhenRequested`
- `OrderBusinessRules.OrderMustExistWhenRequested`
- `StockBusinessRules.StockMustExistWhenRequested`
- `SaleBusinessRules.SaleMustExistWhenRequested`

---

_Bu dokümantasyon PharmacyDepot projesinin `CrossCuttingConcerns` katmanındaki global hata yönetimi sistemini kapsamaktadır. Sorularınız için projenin GitHub deposunu inceleyebilirsiniz._
