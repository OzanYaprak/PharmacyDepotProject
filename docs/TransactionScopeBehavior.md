# TransactionScopeBehavior — Otomatik Transaction Yönetimi

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Mimari Konum](#mimari-konum)
- [Bileşenler](#bileşenler)
  - [ITransactionalRequest](#itransactionalrequest)
  - [TransactionScopeBehavior](#transactionscopebehavior)
  - [Pipeline Kaydı](#pipeline-kaydı)
- [Nasıl Çalışır?](#nasıl-çalışır)
- [Projede Kullanım](#projede-kullanım)
- [MediatR Pipeline Sırası](#mediatr-pipeline-sırası)
- [Dikkat Edilmesi Gerekenler](#dikkat-edilmesi-gerekenler)
- [İlerideki Geliştirmeler](#i̇lerideki-geliştirmeler)

---

## Genel Bakış

`TransactionScopeBehavior`, her **Create / Update / Delete** komutunun otomatik olarak bir veritabanı transaction'ı içinde çalışmasını sağlayan bir **MediatR Pipeline Behavior**'ıdır.

Bu yapı sayesinde:
- Handler içinde `BeginTransaction` / `Commit` / `Rollback` çağrısı yazmaya gerek kalmaz.
- Bir komut başarıyla tamamlanırsa değişiklikler **commit** edilir.
- Herhangi bir exception fırlatılırsa transaction otomatik olarak **rollback** yapılır.
- Tüm bu davranış, `ITransactionalRequest` marker interface'i eklenerek **opt-in** olarak etkinleştirilir.

---

## Mimari Konum

```
Application/
└── Pipelines/
	├── Transaction/
	│   ├── ITransactionalRequest.cs         ← Marker interface
	│   └── TransactionScopeBehavior.cs      ← Pipeline davranışı
	└── Validation/
		└── RequestValidationBehavior.cs
```

Bu dosyalar **Application** katmanına aittir ve altyapı bağımlılığı içermez. `System.Transactions` standart bir .NET kütüphanesidir.

---

## Bileşenler

### ITransactionalRequest

```csharp
// Application/Pipelines/Transaction/ITransactionalRequest.cs
namespace Application.Pipelines.Transaction;

public interface ITransactionalRequest
{
}
```

**Marker interface** — davranışsal kural içermez; sadece bir komutun transaction gerektirdiğini işaretlemek için kullanılır.

Bir komuta `ITransactionalRequest` eklendiğinde, MediatR pipeline'ı `TransactionScopeBehavior`'ı devreye sokar.

---

### TransactionScopeBehavior

```csharp
// Application/Pipelines/Transaction/TransactionScopeBehavior.cs
public class TransactionScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, ITransactionalRequest
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		using TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

		TResponse response;

		try
		{
			response = await next();
			transactionScope.Complete();  // Başarılı → commit
		}
		catch (Exception)
		{
			transactionScope.Dispose();   // Hata → rollback
			throw;
		}

		return response;
	}
}
```

**`TransactionScopeAsyncFlowOption.Enabled`** parametresi kritiktir: `async/await` kullanılan ortamda transaction bağlamının doğru thread'e aktarılmasını sağlar. Bu olmadan async kod içinde transaction kaybedilebilir.

---

### Pipeline Kaydı

```csharp
// Application/ApplicationServiceRegistration.cs
services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

	cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));  // 1. Validation
	cfg.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));   // 2. Transaction
});
```

Kayıt sırası önemlidir: **önce validation çalışır**, geçersiz istekler transaction açılmadan reddedilir. Bu sayede gereksiz transaction yükü oluşmaz.

---

## Nasıl Çalışır?

```
Controller
	│
	▼
ISender.Send(command)
	│
	▼
┌─────────────────────────────────┐
│  RequestValidationBehavior      │  ← FluentValidation kuralları çalışır
│  (ValidationException fırlatır) │    Hata varsa burada durur
└────────────┬────────────────────┘
			 │
			 ▼
┌─────────────────────────────────┐
│  TransactionScopeBehavior       │  ← TransactionScope açılır
│  (sadece ITransactionalRequest  │
│   implement eden komutlar için) │
└────────────┬────────────────────┘
			 │
			 ▼
┌─────────────────────────────────┐
│  Handler                        │  ← İş mantığı çalışır
│  (örn. CreateDrugCommandHandler)│    SaveChanges() burada çağrılır
└────────────┬────────────────────┘
			 │
		┌────┴─────┐
		│ Başarılı │──→ transactionScope.Complete() → COMMIT
		│  Hata    │──→ transactionScope.Dispose()  → ROLLBACK + Exception yeniden fırlatılır
		└──────────┘
```

---

## Projede Kullanım

Aşağıdaki tüm komutlar `ITransactionalRequest` implement ederek otomatik transaction koruması altındadır:

| Entity | Create | Update | Delete |
|---|:---:|:---:|:---:|
| **Customer** | ✅ | ✅ | ✅ |
| **Drug** | ✅ | ✅ | ✅ |
| **Order** | ✅ | ✅ | ✅ |
| **Sale** | ✅ | ✅ | ✅ |
| **Stock** | ✅ | ✅ | ✅ |
| **Supplier** | ✅ | ✅ | ✅ |
| **Warehouse** | ✅ | ✅ | ✅ |

**Query komutları** (GetList, GetById vb.) `ITransactionalRequest` **implement etmez** — veri okuyan sorgular için transaction gerekmez.

Yeni bir komuta transaction eklemek için tek yapılması gereken:

```csharp
// ÖNCE — transaction yok
public class CreateXyzCommand : IRequest<CreatedXyzResponse>

// SONRA — transaction aktif
public class CreateXyzCommand : IRequest<CreatedXyzResponse>, ITransactionalRequest
```

---

## MediatR Pipeline Sırası

```
ValidationBehavior → TransactionScopeBehavior → Handler
```

Bu sırayla:
1. **Validation** geçersiz istekleri erken yakalar, transaction maliyeti oluşmaz.
2. **Transaction** yalnızca geçerli istekler için açılır.
3. **Handler** temiz bir transaction bağlamı içinde çalışır.

Gelecekte yeni bir behavior eklenirken (örn. `LoggingBehavior`, `CachingBehavior`) sıranın önemi göz önünde bulundurulmalıdır.

---

## Dikkat Edilmesi Gerekenler

| Konu | Açıklama |
|---|---|
| **Async akış** | `TransactionScopeAsyncFlowOption.Enabled` mutlaka kullanılmalıdır; aksi hâlde `await` sonrası transaction kaybolur. |
| **Distributed Transaction** | `System.Transactions.TransactionScope`, varsayılan olarak tek bir veritabanı bağlantısını kapsar. Birden fazla veritabanı veya harici servis içeren işlemlerde farklı bir yaklaşım gerekir. |
| **Uzun transaction'lar** | Handler içinde ağ/IO çağrısı varsa transaction açık kaldığı süre uzar ve lock sorunlarına yol açabilir. Harici servis çağrıları transaction dışına alınmalıdır. |
| **Nested transaction** | `TransactionScope` iç içe çalışmayı destekler (`TransactionScopeOption.Required` varsayılan); ancak bilinçli olmadan iç içe kullanım beklenmedik davranışa neden olabilir. |
| **SaveChanges** | EF Core `SaveChanges()` / `SaveChangesAsync()` yalnızca handler içinde çağrılır; behavior bunu tetiklemez. |

---

## İlerideki Geliştirmeler

### 1. Loglama Behavior'ı Eklemek

Transaction'ların başlangıç/bitiş sürelerini ve sonuçlarını izlemek için `LoggingBehavior` eklenebilir:

```csharp
// Application/Pipelines/Logging/LoggingBehavior.cs
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
		=> _logger = logger;

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
		var response = await next();
		_logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
		return response;
	}
}
```

Pipeline sırası: `Logging → Validation → Transaction → Handler`

---

### 2. Retry / Idempotency Behavior'ı

Geçici veritabanı hatalarında (deadlock, connection timeout) otomatik yeniden deneme için `RetryBehavior` eklenebilir. `Polly` kütüphanesi bu iş için uygundur:

```csharp
// Yalnızca ITransactionalRequest olan komutlar retry'a aday olabilir
// Idempotency key ile aynı isteğin iki kez işlenmesi engellenebilir
public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, ITransactionalRequest
```

---

### 3. Unit of Work Entegrasyonu

Şu an `SaveChanges()` her handler'da ayrı ayrı çağrılıyor. **Unit of Work** deseni ile bu merkeze alınabilir:

```csharp
// IUnitOfWork repository katmanında tanımlanır
public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// TransactionScopeBehavior içinde SaveChanges otomatik çağrılabilir:
response = await next();
await _unitOfWork.SaveChangesAsync(cancellationToken); // handler'da çağırmaya gerek kalmaz
transactionScope.Complete();
```

Bu yaklaşım handler'ları daha sade tutar ve `SaveChanges` çağrısını unutma riskini ortadan kaldırır.

---

### 4. Saga / Outbox Pattern

`Order` → `Stock` → `Sale` gibi birden fazla aggregate'i etkileyen işlemler için basit `TransactionScope` yetersiz kalabilir. Bu tür senaryolarda:

- **Outbox Pattern**: Veritabanına event kaydedip, ayrı bir worker'ın bunu işlemesi
- **Saga Pattern**: MassTransit veya benzeri bir araçla dağıtık transaction yönetimi

Bu geliştirme ihtiyacı, özellikle `CreateOrderCommand` handler'ı stok ve satış kaydını da tetiklemeye başlarsa gündeme gelecektir.

---

### 5. İzolasyon Seviyesi Kontrolü

Varsayılan `TransactionScope` izolasyon seviyesi `ReadCommitted`'dir. Özel durumlar için override edilebilir:

```csharp
// ITransactionalRequest'e izolasyon seviyesi eklenerek granüler kontrol sağlanabilir
public interface ITransactionalRequest
{
	IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted; // default
}

// Kritik stock güncelleme komutları için:
public class UpdateStockCommand : IRequest<UpdatedStockResponse>, ITransactionalRequest
{
	public IsolationLevel IsolationLevel => IsolationLevel.RepeatableRead;
	// ... diğer property'ler
}
```

---

### 6. Test Stratejisi

`TransactionScopeBehavior` için yazılabilecek testler:

```csharp
// Application.Tests/Pipelines/Transaction/TransactionScopeBehaviorTests.cs

// ✅ Başarılı handler → commit beklenir (exception fırlatılmamalı)
[Fact]
public async Task Handle_WhenHandlerSucceeds_ShouldComplete()

// ✅ Handler exception fırlatırsa → rollback ve exception yeniden fırlatılır
[Fact]
public async Task Handle_WhenHandlerThrows_ShouldRollbackAndRethrow()
```

> Handler gerçek bir veritabanına bağlanmadan test edilmek istenirse `InMemory` EF Core provider kullanılabilir; ancak `TransactionScope` ile InMemory provider tam uyumlu değildir. Bu durumda integration test ortamı (örn. Testcontainers + PostgreSQL/SQL Server) tercih edilmelidir.
