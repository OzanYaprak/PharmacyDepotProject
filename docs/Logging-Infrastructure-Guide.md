# 📋 Loglama Altyapısı — Kapsamlı Rehber

> Bu doküman, PharmacyDepot projesindeki loglama altyapısını derinlemesine açıklar ve aynı yapıyı başka projelerde sıfırdan nasıl kuracağınızı adım adım gösterir.

---

## İçindekiler

1. [Mimari Genel Bakış](#1-mimari-genel-bakış)
2. [Bileşenler ve Sorumlulukları](#2-bileşenler-ve-sorumlulukları)
3. [Veri Modelleri](#3-veri-modelleri)
4. [Serilog Logger Servisleri](#4-serilog-logger-servisleri)
5. [MediatR Pipeline — LoggingBehavior](#5-mediatr-pipeline--loggingbehavior)
6. [Konfigürasyon](#6-konfigürasyon)
7. [DI Kaydı](#7-di-kaydı)
8. [Loglama Akışı — Uçtan Uca](#8-loglama-akışı--uçtan-uca)
9. [Başka Bir Projede Sıfırdan Kurulum](#9-başka-bir-projede-sıfırdan-kurulum)
10. [Sık Karşılaşılan Hatalar](#10-sık-karşılaşılan-hatalar)

---

## 1. Mimari Genel Bakış

Loglama altyapısı üç farklı katmana yayılmıştır:

```
┌─────────────────────────────────────────────────────────────┐
│  WebAPI                                                      │
│  └─ Program.cs → DI kayıtları (Logger seçimi burada yapılır)│
├─────────────────────────────────────────────────────────────┤
│  Application                                                 │
│  └─ Pipelines/Logging/                                       │
│       ├─ ILoggableRequest.cs   (marker interface)            │
│       └─ LoggingBehavior.cs    (MediatR pipeline)            │
├─────────────────────────────────────────────────────────────┤
│  CrossCuttingConcerns  (bağımsız loglama kütüphanesi)        │
│  ├─ Logging/                                                 │
│  │    ├─ LogDetail.cs                                        │
│  │    ├─ LogDetailWithException.cs                           │
│  │    └─ LogParameter.cs                                     │
│  └─ Serilog/                                                 │
│       ├─ LoggerServiceBase.cs                                │
│       ├─ Messages/SerilogMessages.cs                         │
│       ├─ ConfigurationModels/                                │
│       │    ├─ FileLogConfiguration.cs                        │
│       │    └─ MssqlConfiguration.cs                          │
│       └─ Loggers/                                            │
│            ├─ FileLogger.cs                                  │
│            └─ MssqlLogger.cs                                 │
└─────────────────────────────────────────────────────────────┘
```

**Temel Prensipler:**
- `CrossCuttingConcerns` katmanı hiçbir domain/application sınıfına bağımlı değildir; her projede yeniden kullanılabilir.
- Logger seçimi (`FileLogger` vs `MssqlLogger`) tek satır değişiklikle yapılır.
- MediatR pipeline üzerinden çalışır; controller veya handler kodu değişmeden loglama aktif olur.
- Loglama isteğe bağlıdır: yalnızca `ILoggableRequest` implemente eden Command/Query'ler loglanır.

---

## 2. Bileşenler ve Sorumlulukları

| Bileşen | Katman | Sorumluluk |
|---|---|---|
| `LogDetail` | CrossCuttingConcerns | Tek bir log kaydının veri modelini tanımlar |
| `LogDetailWithException` | CrossCuttingConcerns | Hata içeren log kaydı modeli (`LogDetail`'den türer) |
| `LogParameter` | CrossCuttingConcerns | Log kaydındaki tek bir parametre (isim, değer, tip) |
| `LoggerServiceBase` | CrossCuttingConcerns | Tüm logger'ların ortak soyut tabanı; Serilog metodlarını sarar |
| `FileLogger` | CrossCuttingConcerns | Logları `.txt` ve `.json` dosyalarına yazar |
| `MssqlLogger` | CrossCuttingConcerns | Logları SQL Server tablosuna yazar |
| `SerilogMessages` | CrossCuttingConcerns | Sabit hata mesajları (magic string önleme) |
| `ILoggableRequest` | Application | Marker interface; hangi request'lerin loglanacağını belirtir |
| `LoggingBehavior<TRequest,TResponse>` | Application | MediatR pipeline behavior; loglama iş mantığını içerir |

---

## 3. Veri Modelleri

### `LogParameter`
Bir request'in parametresini temsil eder.

```csharp
// CrossCuttingConcerns/Logging/LogParameter.cs
public class LogParameter
{
	public string Name { get; set; }   // Parametre adı
	public object Value { get; set; }  // Parametre değeri (JSON'a serialize edilir)
	public string Type { get; set; }   // .NET tip adı
}
```

### `LogDetail`
Bir MediatR request'inin tam log kaydı.

```csharp
// CrossCuttingConcerns/Logging/LogDetail.cs
public class LogDetail
{
	public string? Fullname { get; set; }         // Namespace + sınıf adı
	public string? MethodName { get; set; }       // HTTP metodu + path + request adı
	public string? User { get; set; }             // Kullanıcı adı (anonymous ise "Anonymous")
	public List<LogParameter>? Parameters { get; set; }  // Request parametreleri
	public double ExecutionTimeMs { get; set; }   // İşlem süresi (ms)
}
```

### `LogDetailWithException`
`LogDetail`'i miras alır, üzerine hata mesajı ekler.

```csharp
// CrossCuttingConcerns/Logging/LogDetailWithException.cs
public class LogDetailWithException : LogDetail
{
	public string? ExceptionMessage { get; set; }  // Exception mesajı
}
```

> **Not:** `LogDetailWithException` şu an `LoggingBehavior` içinde doğrudan kullanılmıyor; hata durumunda `LogDetail` serialize edilip `Error` seviyesinde yazılıyor. Bu sınıf daha zengin hata loglaması için genişletilebilir.

---

## 4. Serilog Logger Servisleri

### `LoggerServiceBase` — Soyut Taban

Tüm logger'ların türediği temel sınıf. Serilog'un `ILogger` nesnesini sarar ve 6 seviyeli log metodu sunar:

```csharp
// CrossCuttingConcerns/Serilog/LoggerServiceBase.cs
public abstract class LoggerServiceBase
{
	protected ILogger? Logger { get; set; }

	public void Verbose(string message) => Logger?.Verbose(message);
	public void Fatal(string message)   => Logger?.Fatal(message);
	public void Info(string message)    => Logger?.Information(message);
	public void Warning(string message) => Logger?.Warning(message);
	public void Debug(string message)   => Logger?.Debug(message);
	public void Error(string message)   => Logger?.Error(message);
}
```

**Neden soyut sınıf?**
- Ortak metotları tek yerde tanımlar.
- Yeni bir logger eklemek için yalnızca constructor'ı farklı `LoggerConfiguration` ile doldurmak yeterlidir.

---

### `FileLogger` — Dosyaya Loglama

```csharp
// CrossCuttingConcerns/Serilog/Loggers/FileLogger.cs
public class FileLogger : LoggerServiceBase
{
	public FileLogger(IConfiguration configuration)
	{
		FileLogConfiguration config = configuration
			.GetSection("SeriLogConfigurations:FileLogConfiguration")
			.Get<FileLogConfiguration>()
			?? throw new InvalidOperationException(SerilogMessages.NullOptionsMessage);

		string logFolder = Path.Combine(Directory.GetCurrentDirectory(), config.FolderPath);

		Logger = new LoggerConfiguration()
			// Düz metin dosyası
			.WriteTo.File(
				path: Path.Combine(logFolder, "Logs.txt"),
				rollingInterval: RollingInterval.Hour,   // Saatlik yeni dosya
				retainedFileCountLimit: null,            // Sınırsız dosya sakla
				fileSizeLimitBytes: 5_000_000,           // Maks 5 MB
				outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
			// JSON formatı (makine tarafından okunabilir)
			.WriteTo.File(
				formatter: new JsonFormatter(renderMessage: true),
				path: Path.Combine(logFolder, "Logs.json"),
				rollingInterval: RollingInterval.Hour,
				retainedFileCountLimit: null,
				fileSizeLimitBytes: 5_000_000)
			.CreateLogger();
	}
}
```

**Üretilen dosya isimleri:**
- `Logs2026052714.txt` → 2026-05-27 saat 14:xx logları
- `Logs2026052714.json` → aynı saatin JSON karşılığı

---

### `MssqlLogger` — SQL Server'a Loglama

```csharp
// CrossCuttingConcerns/Serilog/Loggers/MssqlLogger.cs
public class MssqlLogger : LoggerServiceBase
{
	public MssqlLogger(IConfiguration configuration)
	{
		MssqlConfiguration config = configuration
			.GetSection("SeriLogConfigurations:MssqlLogConfiguration")
			.Get<MssqlConfiguration>()
			?? throw new InvalidOperationException(SerilogMessages.NullOptionsMessage);

		MSSqlServerSinkOptions sinkOptions = new()
		{
			TableName = config.TableName,           // "Logs"
			AutoCreateSqlTable = config.AutoCreateSqlTable  // true → tablo yoksa oluşturur
		};

		Logger = new LoggerConfiguration()
			.WriteTo.MSSqlServer(
				connectionString: config.ConnectionString,
				sinkOptions: sinkOptions,
				columnOptions: new ColumnOptions())
			.CreateLogger();
	}
}
```

**`AutoCreateSqlTable: true` iken oluşan tablo şeması (Serilog varsayılanı):**

| Kolon | Tip | Açıklama |
|---|---|---|
| Id | int (PK) | Otomatik artan |
| Message | nvarchar(max) | Log mesajı |
| MessageTemplate | nvarchar(max) | Serilog şablonu |
| Level | nvarchar(128) | Information, Error, vb. |
| TimeStamp | datetime | Log zamanı |
| Exception | nvarchar(max) | Exception detayı (varsa) |
| Properties | nvarchar(max) | Ek özellikler (XML) |

---

## 5. MediatR Pipeline — LoggingBehavior

`LoggingBehavior`, her MediatR request'ini intercepter gibi yakalar; başında ve sonunda log yazar.

```csharp
// Application/Pipelines/Logging/LoggingBehavior.cs
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, ILoggableRequest   // ← marker interface şartı
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly LoggerServiceBase _loggerServiceBase;

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var stopwatch = Stopwatch.StartNew();

		// 1. Request parametrelerini topla
		List<LogParameter> logParameters = new()
		{
			new LogParameter
			{
				Name  = typeof(TRequest).Name,
				Type  = typeof(TRequest).Name,
				Value = request                 // Tüm command/query nesnesi JSON'a serialize edilir
			}
		};

		// 2. HTTP context bilgilerini al
		var httpContext   = _httpContextAccessor.HttpContext;
		var user          = httpContext?.User?.Identity?.Name;
		var requestPath   = httpContext?.Request?.Path.Value ?? string.Empty;
		var requestMethod = httpContext?.Request?.Method ?? string.Empty;

		// 3. LogDetail oluştur
		LogDetail logDetail = new()
		{
			Fullname   = $"{typeof(TRequest).Namespace}.{typeof(TRequest).Name}",
			MethodName = $"[{requestMethod}] {requestPath} {typeof(TRequest).Name}",
			Parameters = logParameters,
			User       = string.IsNullOrWhiteSpace(user) ? "Anonymous" : user
		};

		try
		{
			// 4. Başlangıç logu
			_loggerServiceBase.Info($"[START] {logDetail.Fullname} initiated by user: {logDetail.User}");

			TResponse response = await next();  // Handler çalışır

			// 5. Başarı logu (süre dahil)
			stopwatch.Stop();
			logDetail.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
			_loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));

			return response;
		}
		catch (Exception)
		{
			// 6. Hata logu
			stopwatch.Stop();
			logDetail.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
			_loggerServiceBase.Error(JsonSerializer.Serialize(logDetail));
			throw;  // Exception'ı yutma, üst katmana ilet
		}
	}
}
```

### ILoggableRequest — Marker Interface

```csharp
// Application/Pipelines/Logging/ILoggableRequest.cs
public interface ILoggableRequest { }
```

Bir Command veya Query'nin loglanmasını istiyorsanız bu interface'i ekleyin:

```csharp
// Loglanmayan command (varsayılan):
public record CreateDrugCommand(string Name, decimal Price) : IRequest<Guid>;

// Loglanacak command:
public record CreateDrugCommand(string Name, decimal Price)
	: IRequest<Guid>, ILoggableRequest;
```

> **Önemli:** `ILoggableRequest` olmayan request'ler `LoggingBehavior`'a hiç girmez.  
> Bu sayede loglama seçici ve performanslıdır.

---

## 6. Konfigürasyon

### `appsettings.json`

```json
{
  "SeriLogConfigurations": {
	"FileLogConfiguration": {
	  "FolderPath": "Logs"
	},
	"MssqlLogConfiguration": {
	  "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=PharmacyDepotDb;Trusted_Connection=True;",
	  "TableName": "Logs",
	  "AutoCreateSqlTable": true
	}
  }
}
```

**`FileLogConfiguration.FolderPath`:**  
Uygulamanın çalışma dizinine (`Directory.GetCurrentDirectory()`) göre göreceli yol.  
`"Logs"` → `WebAPI/Logs/` klasörü oluşturulur.

---

## 7. DI Kaydı

### Logger Seçimi (`ApplicationServiceRegistration.cs`)

```csharp
// Dosyaya logla:
services.AddSingleton<LoggerServiceBase, FileLogger>();

// SQL Server'a logla:
services.AddSingleton<LoggerServiceBase, MssqlLogger>();
```

> Logger `Singleton` olarak kayıtlanır. Serilog yazıcıları (sink) thread-safe olduğundan bu güvenlidir.

### `IHttpContextAccessor` Kaydı (`Program.cs`)

`LoggingBehavior` HTTP context bilgilerine (kullanıcı adı, path, metot) erişmek için `IHttpContextAccessor` kullanır. Bu servisin kayıtlı olması zorunludur:

```csharp
builder.Services.AddHttpContextAccessor();
```

### MediatR Pipeline Sırası

```csharp
cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));  // 1. Doğrulama
cfg.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));   // 2. Transaction
cfg.AddOpenBehavior(typeof(CachingBehavior<,>));            // 3. Cache okuma
cfg.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));      // 4. Cache temizleme
cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));            // 5. Loglama (en son)
```

> Loglama en son pipeline'a eklenir; bu sayede gerçek işlem süresini (`ExecutionTimeMs`) ölçebilir.

---

## 8. Loglama Akışı — Uçtan Uca

```
HTTP Request (POST /api/drugs)
		│
		▼
[Controller] DrugsController.Create()
		│ _sender.Send(command)
		▼
[MediatR Pipeline]
  ┌─ RequestValidationBehavior  → FluentValidation çalışır
  ├─ TransactionScopeBehavior   → Transaction başlar
  ├─ CachingBehavior            → Cache kontrol edilir
  ├─ CacheRemovingBehavior      → Cache temizlenir
  └─ LoggingBehavior            → [Sadece ILoggableRequest ise]
		│
		│  1. Stopwatch başlatılır
		│  2. LogDetail oluşturulur (user, path, parameters)
		│  3. INFO "[START] ..." yazılır
		│
		▼
[Handler] CreateDrugCommandHandler.Handle()
		│
		▼
[LoggingBehavior — devam]
		│  4. Stopwatch durdurulur
		│  5. ExecutionTimeMs hesaplanır
		│  6. INFO (JSON LogDetail) yazılır
		│     ─ HATA olursa → ERROR (JSON LogDetail) yazılır + throw
		▼
[Logger Sink]
  ├─ FileLogger  → Logs/Logs{timestamp}.txt + .json
  └─ MssqlLogger → [DB].dbo.Logs tablosu
```

---

## 9. Başka Bir Projede Sıfırdan Kurulum

Aşağıdaki adımlar, bu loglama altyapısını yeni bir Clean Architecture projesine eklemek için eksiksiz bir rehberdir.

---

### Adım 1 — NuGet Paketlerini Ekle

**`CrossCuttingConcerns` projesine:**
```
Serilog
Serilog.Sinks.File
Serilog.Sinks.MSSqlServer
Microsoft.Extensions.Configuration.Abstractions
```

**`Application` projesine:**
```
MediatR
Microsoft.AspNetCore.Http.Abstractions
```

---

### Adım 2 — CrossCuttingConcerns Katmanı: Klasör Yapısı

```
CrossCuttingConcerns/
├── Logging/
│   ├── LogDetail.cs
│   ├── LogDetailWithException.cs
│   └── LogParameter.cs
└── Serilog/
	├── LoggerServiceBase.cs
	├── Messages/
	│   └── SerilogMessages.cs
	├── ConfigurationModels/
	│   ├── FileLogConfiguration.cs
	│   └── MssqlConfiguration.cs
	└── Loggers/
		├── FileLogger.cs
		└── MssqlLogger.cs
```

**Dosyalar (kopyala-yapıştır):**

```csharp
// Logging/LogParameter.cs
namespace CrossCuttingConcerns.Logging;

public class LogParameter
{
	public LogParameter() { Name = string.Empty; Value = string.Empty; Type = string.Empty; }
	public LogParameter(string name, object value, string type)
	{ Name = name; Value = value; Type = type; }

	public string Name { get; set; }
	public object Value { get; set; }
	public string Type { get; set; }
}
```

```csharp
// Logging/LogDetail.cs
namespace CrossCuttingConcerns.Logging;

public class LogDetail
{
	public LogDetail() { Fullname = string.Empty; MethodName = string.Empty; User = string.Empty; Parameters = new(); }
	public LogDetail(string fullname, string methodName, string user, List<LogParameter> parameters)
	{ Fullname = fullname; MethodName = methodName; User = user; Parameters = parameters; }

	public string? Fullname { get; set; }
	public string? MethodName { get; set; }
	public string? User { get; set; }
	public List<LogParameter>? Parameters { get; set; }
	public double ExecutionTimeMs { get; set; }
}
```

```csharp
// Logging/LogDetailWithException.cs
namespace CrossCuttingConcerns.Logging;

public class LogDetailWithException : LogDetail
{
	public LogDetailWithException() { ExceptionMessage = string.Empty; }
	public LogDetailWithException(string fullname, string methodName, string user, List<LogParameter> parameters, string exceptionMessage)
		: base(fullname, methodName, user, parameters) { ExceptionMessage = exceptionMessage; }

	public string? ExceptionMessage { get; set; }
}
```

```csharp
// Serilog/Messages/SerilogMessages.cs
namespace CrossCuttingConcerns.Serilog.Messages;

public static class SerilogMessages
{
	public static string NullOptionsMessage => "Loglama konfigürasyonu boş geldi. appsettings.json'u kontrol edin.";
}
```

```csharp
// Serilog/ConfigurationModels/FileLogConfiguration.cs
namespace CrossCuttingConcerns.Serilog.ConfigurationModels;

public class FileLogConfiguration
{
	public FileLogConfiguration() { FolderPath = string.Empty; }
	public FileLogConfiguration(string folderPath) { FolderPath = folderPath; }
	public string FolderPath { get; set; }
}
```

```csharp
// Serilog/ConfigurationModels/MssqlConfiguration.cs
namespace CrossCuttingConcerns.Serilog.ConfigurationModels;

public class MssqlConfiguration
{
	public MssqlConfiguration() { ConnectionString = string.Empty; TableName = string.Empty; }
	public MssqlConfiguration(string connectionString, string tableName, bool autoCreateSqlTable)
	{ ConnectionString = connectionString; TableName = tableName; AutoCreateSqlTable = autoCreateSqlTable; }

	public string ConnectionString { get; set; }
	public string TableName { get; set; }
	public bool AutoCreateSqlTable { get; set; }
}
```

```csharp
// Serilog/LoggerServiceBase.cs
using Serilog;

namespace CrossCuttingConcerns.Serilog;

public abstract class LoggerServiceBase
{
	protected LoggerServiceBase() { Logger = null; }
	protected LoggerServiceBase(ILogger logger) { Logger = logger; }

	protected ILogger? Logger { get; set; }

	public void Verbose(string message) => Logger?.Verbose(message);
	public void Fatal(string message)   => Logger?.Fatal(message);
	public void Info(string message)    => Logger?.Information(message);
	public void Warning(string message) => Logger?.Warning(message);
	public void Debug(string message)   => Logger?.Debug(message);
	public void Error(string message)   => Logger?.Error(message);
}
```

```csharp
// Serilog/Loggers/FileLogger.cs
using CrossCuttingConcerns.Serilog.ConfigurationModels;
using CrossCuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Json;

namespace CrossCuttingConcerns.Serilog.Loggers;

public class FileLogger : LoggerServiceBase
{
	public FileLogger(IConfiguration configuration)
	{
		var config = configuration
			.GetSection("SeriLogConfigurations:FileLogConfiguration")
			.Get<FileLogConfiguration>()
			?? throw new InvalidOperationException(SerilogMessages.NullOptionsMessage);

		string logFolder = Path.Combine(Directory.GetCurrentDirectory(), config.FolderPath);

		Logger = new LoggerConfiguration()
			.WriteTo.File(
				path: Path.Combine(logFolder, "Logs.txt"),
				rollingInterval: RollingInterval.Hour,
				retainedFileCountLimit: null,
				fileSizeLimitBytes: 5_000_000,
				outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
			.WriteTo.File(
				formatter: new JsonFormatter(renderMessage: true),
				path: Path.Combine(logFolder, "Logs.json"),
				rollingInterval: RollingInterval.Hour,
				retainedFileCountLimit: null,
				fileSizeLimitBytes: 5_000_000)
			.CreateLogger();
	}
}
```

```csharp
// Serilog/Loggers/MssqlLogger.cs
using CrossCuttingConcerns.Serilog.ConfigurationModels;
using CrossCuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace CrossCuttingConcerns.Serilog.Loggers;

public class MssqlLogger : LoggerServiceBase
{
	public MssqlLogger(IConfiguration configuration)
	{
		var config = configuration
			.GetSection("SeriLogConfigurations:MssqlLogConfiguration")
			.Get<MssqlConfiguration>()
			?? throw new InvalidOperationException(SerilogMessages.NullOptionsMessage);

		Logger = new LoggerConfiguration()
			.WriteTo.MSSqlServer(
				connectionString: config.ConnectionString,
				sinkOptions: new MSSqlServerSinkOptions
				{
					TableName = config.TableName,
					AutoCreateSqlTable = config.AutoCreateSqlTable
				},
				columnOptions: new ColumnOptions())
			.CreateLogger();
	}
}
```

---

### Adım 3 — Application Katmanı: Klasör Yapısı

```
Application/
└── Pipelines/
	└── Logging/
		├── ILoggableRequest.cs
		└── LoggingBehavior.cs
```

```csharp
// Pipelines/Logging/ILoggableRequest.cs
namespace Application.Pipelines.Logging;

public interface ILoggableRequest { }
```

```csharp
// Pipelines/Logging/LoggingBehavior.cs
using CrossCuttingConcerns.Logging;
using CrossCuttingConcerns.Serilog;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.Json;

namespace Application.Pipelines.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, ILoggableRequest
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly LoggerServiceBase _loggerServiceBase;

	public LoggingBehavior(IHttpContextAccessor httpContextAccessor, LoggerServiceBase loggerServiceBase)
	{
		_httpContextAccessor = httpContextAccessor;
		_loggerServiceBase = loggerServiceBase;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var stopwatch = Stopwatch.StartNew();

		var httpContext = _httpContextAccessor.HttpContext;
		var user = httpContext?.User?.Identity?.Name;

		LogDetail logDetail = new()
		{
			Fullname   = $"{typeof(TRequest).Namespace}.{typeof(TRequest).Name}",
			MethodName = $"[{httpContext?.Request?.Method}] {httpContext?.Request?.Path.Value} {typeof(TRequest).Name}",
			Parameters = new List<LogParameter>
			{
				new() { Name = typeof(TRequest).Name, Type = typeof(TRequest).Name, Value = request }
			},
			User = string.IsNullOrWhiteSpace(user) ? "Anonymous" : user
		};

		try
		{
			_loggerServiceBase.Info($"[START] {logDetail.Fullname} initiated by user: {logDetail.User}");

			var response = await next();

			stopwatch.Stop();
			logDetail.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
			_loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));

			return response;
		}
		catch (Exception)
		{
			stopwatch.Stop();
			logDetail.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
			_loggerServiceBase.Error(JsonSerializer.Serialize(logDetail));
			throw;
		}
	}
}
```

---

### Adım 4 — DI Kayıtlarını Yap

**`Application/ApplicationServiceRegistration.cs`** içine ekle:

```csharp
using CrossCuttingConcerns.Serilog;
using CrossCuttingConcerns.Serilog.Loggers;
using Application.Pipelines.Logging;

// MediatR pipeline'a ekle (AddOpenBehavior listesinin sonuna):
cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

// Logger seçimi (birini aktif bırak):
services.AddSingleton<LoggerServiceBase, FileLogger>();
// services.AddSingleton<LoggerServiceBase, MssqlLogger>();
```

**`Program.cs`** içine ekle:

```csharp
builder.Services.AddHttpContextAccessor();
```

---

### Adım 5 — appsettings.json'a Konfigürasyon Ekle

```json
{
  "SeriLogConfigurations": {
	"FileLogConfiguration": {
	  "FolderPath": "Logs"
	},
	"MssqlLogConfiguration": {
	  "ConnectionString": "Server=.;Database=MyDb;Trusted_Connection=True;",
	  "TableName": "Logs",
	  "AutoCreateSqlTable": true
	}
  }
}
```

---

### Adım 6 — Command/Query'leri Loglanacak Şekilde İşaretle

```csharp
// Loglamak istediğiniz Command:
public record CreateProductCommand(string Name, decimal Price)
	: IRequest<Guid>, ILoggableRequest;

// Loglamak istediğiniz Query:
public record GetByIdProductQuery(Guid Id)
	: IRequest<ProductDto>, ILoggableRequest;
```

---

### Adım 7 — Proje Referanslarını Kontrol Et

```
WebAPI           → CrossCuttingConcerns, Application
Application      → CrossCuttingConcerns
CrossCuttingConcerns → (harici bağımlılık yok — yalnızca NuGet paketleri)
```

---

## 10. Sık Karşılaşılan Hatalar

| Hata | Neden | Çözüm |
|---|---|---|
| `InvalidOperationException: Loglama konfigürasyonu boş geldi` | `appsettings.json`'da `SeriLogConfigurations` bölümü eksik veya hatalı yazılmış | Section adını ve JSON yapısını kontrol edin |
| `LoggingBehavior` hiç tetiklenmiyor | Command `ILoggableRequest` implemente etmiyor | `ILoggableRequest` interface'ini ekleyin |
| `LoggingBehavior` constructor inject edilemiyor | `IHttpContextAccessor` DI'ya kayıtlı değil | `builder.Services.AddHttpContextAccessor()` ekleyin |
| SQL Server tablosu oluşturulmuyor | `AutoCreateSqlTable: false` veya bağlantı hatası | `AutoCreateSqlTable: true` yapın, connection string'i doğrulayın |
| Loglar `WebAPI/Logs/` yerine farklı yere yazılıyor | `Directory.GetCurrentDirectory()` çalışma dizinine göre çözümleniyor | `FolderPath` değerini mutlak yol olarak verin veya `ContentRootPath` kullanın |
| Log dosyası sürekli büyüyor | `retainedFileCountLimit: null` → eski dosyalar silinmiyor | `retainedFileCountLimit: 30` gibi bir değer verin |

---

## Özet Kontrol Listesi

Yeni projede loglama altyapısını kurmak için:

- [ ] NuGet: `Serilog`, `Serilog.Sinks.File`, `Serilog.Sinks.MSSqlServer`
- [ ] `CrossCuttingConcerns/Logging/` → 3 model sınıfı
- [ ] `CrossCuttingConcerns/Serilog/` → `LoggerServiceBase`, 2 config model, 2 logger
- [ ] `Application/Pipelines/Logging/` → `ILoggableRequest`, `LoggingBehavior`
- [ ] `ApplicationServiceRegistration.cs` → `AddOpenBehavior` + `AddSingleton<LoggerServiceBase>`
- [ ] `Program.cs` → `AddHttpContextAccessor()`
- [ ] `appsettings.json` → `SeriLogConfigurations` bölümü
- [ ] Loglanacak Command/Query'lere `ILoggableRequest` ekle
