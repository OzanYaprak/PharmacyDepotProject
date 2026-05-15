# 📚 FluentValidation & Clean Architecture — Kapsamlı Öğrenme Rehberi

> **Hedef kitle:** .NET dünyasına yeni adım atanlar  
> **Proje bağlamı:** PharmacyDepot — Clean Architecture + MediatR + FluentValidation

---

## 1. Validasyon (Doğrulama) Nedir ve Neden Gereklidir?

Bir kullanıcı API'ye veri gönderdiğinde şu soruları sormamız gerekir:

- Ad alanı gerçekten dolu mu?
- E-posta geçerli bir formatta mı?
- Fiyat negatif olabilir mi?

Eğer bu kontrolleri yapmazsak veritabanına **çöp veri** yazılır, iş kuralları çöker ve
sistemin güvenilirliği sarsılır.

**Validasyonun iki düzeyi vardır:**

| Düzey | Ne Kontrol Eder? | Örnek |
|---|---|---|
| **Format/Girdi Validasyonu** | Verinin şekli, tipi, boşluğu | Email formatı, zorunlu alan, max uzunluk |
| **Business Rule (İş Kuralı)** | Verinin anlam bütünlüğü | "Bu ilaç zaten kayıtlı mı?" |

> FluentValidation → **Format/Girdi Validasyonu**  
> BusinessRules sınıfları → **İş Kuralı Validasyonu**

---

## 2. FluentValidation Nedir?

FluentValidation, .NET için geliştirilmiş popüler bir doğrulama kütüphanesidir.
`if` blokları yerine **akıcı (fluent) API** ile kurallar tanımlamanızı sağlar.

### Geleneksel Yöntem vs FluentValidation

```csharp
// ❌ Geleneksel — dağınık, test edilmesi zor
if (string.IsNullOrEmpty(command.Name))
    throw new Exception("Ad boş olamaz");
if (command.Name.Length > 100)
    throw new Exception("Ad çok uzun");

// ✅ FluentValidation — temiz, okunabilir, test edilebilir
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Ad boş olamaz.")
    .MaximumLength(100).WithMessage("Ad 100 karakteri geçemez.");
```

---

## 3. Temel FluentValidation Kuralları (Cheat Sheet)

### 3.1 Zorunluluk ve Boşluk Kontrolleri

```csharp
RuleFor(x => x.Name).NotNull();      // null olamaz
RuleFor(x => x.Name).NotEmpty();     // null, "" veya "   " olamaz
RuleFor(x => x.Name).Empty();        // boş OLMALI (nadir)
```

### 3.2 Uzunluk Kontrolleri

```csharp
RuleFor(x => x.Name).MinimumLength(3);        // en az 3 karakter
RuleFor(x => x.Name).MaximumLength(100);      // en fazla 100 karakter
RuleFor(x => x.Name).Length(3, 100);          // 3 ile 100 karakter arası
RuleFor(x => x.Name).Length(10);              // tam olarak 10 karakter
```

### 3.3 Sayısal Kontroller

```csharp
RuleFor(x => x.Price).GreaterThan(0);              // 0'dan büyük
RuleFor(x => x.Price).GreaterThanOrEqualTo(0);     // 0 veya büyük
RuleFor(x => x.Price).LessThan(10000);             // 10000'den küçük
RuleFor(x => x.Price).InclusiveBetween(1, 9999);   // 1 ile 9999 arasında (dahil)
RuleFor(x => x.Price).ExclusiveBetween(0, 10000);  // 0 ile 10000 arasında (hariç)
```

### 3.4 Format Kontrolleri

```csharp
RuleFor(x => x.Email).EmailAddress();                     // geçerli email formatı
RuleFor(x => x.Phone).Matches(@"^\+?[0-9]{10,13}$");     // regex ile telefon formatı
RuleFor(x => x.Url).Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute));
```

### 3.5 Koşullu Kurallar

```csharp
// Sadece DiscountRate > 0 ise MaxDiscount kuralını kontrol et
RuleFor(x => x.MaxDiscount)
    .GreaterThan(x => x.DiscountRate)
    .When(x => x.DiscountRate > 0);
```

### 3.6 Özel (Custom) Kurallar

```csharp
RuleFor(x => x.LicenseNumber)
    .Must(license => license.StartsWith("TR"))
    .WithMessage("Ruhsat numarası 'TR' ile başlamalıdır.");

// Async custom kural (veritabanı sorgusu gibi)
RuleFor(x => x.Email)
    .MustAsync(async (email, cancellation) => 
        !await _userRepo.ExistsAsync(email, cancellation))
    .WithMessage("Bu e-posta zaten kullanımda.");
```

---

## 4. Projedeki Mimari: Validation Nasıl Akıyor?

```
HTTP İsteği (POST /api/customers)
          │
          ▼
  ┌─────────────────┐
  │   Controller    │  → ISender.Send(new CreateCustomerCommand(...))
  └────────┬────────┘
           │
           ▼
  ┌──────────────────────────────────┐
  │  RequestValidationBehavior       │  ← MediatR Pipeline Behavior
  │  (Application/Pipelines/...)     │
  │                                  │
  │  1. Tüm IValidator<TCommand>    │
  │     bulunur (DI'dan)            │
  │  2. Her validator çalıştırılır  │
  │  3. Hata var → ValidationException fırlat │
  │  4. Hata yok → next() çağır    │
  └───────────┬──────────────────────┘
              │ Hata varsa ↙  Hata yoksa ↘
              ▼                          ▼
  ┌───────────────────┐      ┌──────────────────────┐
  │ ExceptionMiddleware│      │  CommandHandler       │
  │ yakalar           │      │  (iş mantığı çalışır) │
  └────────┬──────────┘      └──────────────────────┘
           ▼
  ┌──────────────────────────┐
  │  HttpExceptionHandler    │
  │  ValidationHandleException│
  └──────────┬───────────────┘
             ▼
  ┌───────────────────────────┐
  │  HTTP 400 Response        │
  │  ValidationProblemDetails │
  │  (RFC 7807 JSON)          │
  └───────────────────────────┘
```

---

## 5. Dosya Yapısı ve Her Dosyanın Rolü

```
PharmacyDepot/
│
├── Application/
│   ├── Pipelines/
│   │   └── Validation/
│   │       └── RequestValidationBehavior.cs  ← MediatR'a bağlanan köprü
│   │
│   └── Features/
│       └── Customers/
│           └── Commands/
│               └── Create/
│                   ├── CreateCustomerCommand.cs           ← Doğrulanacak veri
│                   └── CreateCustomerCommandValidator.cs  ← Kurallar
│
└── CrossCuttingConcerns/
    └── Exceptions/
        ├── Types/
        │   └── ValidationException.cs          ← Hataları taşıyan exception
        ├── HttpProblemDetails/
        │   └── ValidationProblemDetails.cs     ← HTTP yanıt formatı (RFC 7807)
        ├── Handlers/
        │   ├── ExceptionHandler.cs             ← Soyut base (Template Method deseni)
        │   └── HttpExceptionHandler.cs         ← HTTP'e özel somut implementasyon
        └── Middlewares/
            └── ExceptionMiddleware.cs          ← Exception'ları yakalayan middleware
```

---

## 6. Tasarım Desenleri (Design Patterns) — Neyi Neden Kullandık?

### 6.1 Template Method Pattern (ExceptionHandler)

```
ExceptionHandler (abstract)
    │
    ├── HandleExceptionAsync()       ← Algoritma iskeleti (public, değiştirilemez)
    │       ↓ switch expression
    │   HandleException(Business)    ← abstract (alt sınıf doldurur)
    │   HandleException(NotFound)    ← abstract (alt sınıf doldurur)
    │   HandleException(Validation)  ← abstract (alt sınıf doldurur)
    │   HandleException(Exception)   ← abstract (alt sınıf doldurur)
    │
    └── HttpExceptionHandler : ExceptionHandler
            └── Her abstract metodu override eder (HTTP'e özgü davranış)
```

**Avantajı:** Yarın `ConsoleExceptionHandler` veya `GrpcExceptionHandler` eklemek istersen,
sadece `ExceptionHandler`'dan türet ve metodları override et. Mevcut kod değişmez.

### 6.2 Pipeline Behavior Pattern (RequestValidationBehavior)

MediatR'ın bize sunduğu `IPipelineBehavior<TRequest, TResponse>` arayüzü,
AOP (Aspect-Oriented Programming) fikrinden ilham alır:
**"Her işlemden önce/sonra ortak bir şey yap"** prensibi.

Bu sayede:
- Her handler'a `if (!IsValid) throw` yazmak zorunda kalmazsın
- Validation mantığı tek bir yerde yaşar → DRY prensibi

### 6.3 Factory/Registry Pattern (DI + AbstractValidator)

```
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

Bu tek satır, projenizdeki **tüm** `AbstractValidator<T>` alt sınıflarını otomatik bulur ve
DI container'a kaydeder. Manuel kayıt gerekmez.

---

## 7. Her Yeni Entity İçin Validation Ekleme Adımları

Diyelim ki yeni bir `Drug` (İlaç) entity'si için Create komutu yazacaksınız:

### Adım 1: Validator Sınıfı Oluştur

```
Application/Features/Drugs/Commands/Create/CreateDrugCommandValidator.cs
```

```csharp
public class CreateDrugCommandValidator : AbstractValidator<CreateDrugCommand>
{
    public CreateDrugCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("İlaç adı boş olamaz.")
            .MaximumLength(200).WithMessage("İlaç adı 200 karakteri geçemez.");

        RuleFor(x => x.ActiveIngredient)
            .NotEmpty().WithMessage("Etken madde boş olamaz.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");
    }
}
```

### Adım 2: Başka Bir Şey Yapmanıza Gerek Yok!

`AddValidatorsFromAssembly` ve `AddOpenBehavior(typeof(RequestValidationBehavior<,>))`
sayesinde bu validator otomatik devreye girer.

---

## 8. Örnek HTTP Yanıtları

### Başarılı İstek (HTTP 201)

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Validation Hatası (HTTP 400)

```json
{
  "title": "Validation Failed",
  "detail": "One or more validation errors occurred.",
  "status": 400,
  "type": "https://example.com/probs/validation-error",
  "errors": [
    {
      "propertyName": "Name",
      "errorMessages": ["Müşteri adı boş olamaz."]
    },
    {
      "propertyName": "Email",
      "errorMessages": ["E-posta adresi boş olamaz.", "Geçerli bir e-posta adresi giriniz."]
    }
  ]
}
```

### Business Rule Hatası (HTTP 400)

```json
{
  "title": "Business Rule Violation",
  "detail": "Bu müşteri zaten kayıtlı.",
  "status": 400
}
```

### Kayıt Bulunamadı (HTTP 404)

```json
{
  "title": "Not Found",
  "detail": "Id=99 olan müşteri bulunamadı.",
  "status": 404
}
```

---

## 9. Sık Yapılan Hatalar ve Çözümleri

| Hata | Sebep | Çözüm |
|---|---|---|
| Validator çalışmıyor | `AddValidatorsFromAssembly` eklenmemiş | `ApplicationServiceRegistration.cs` kontrol et |
| Validator çalışıyor ama middleware yakalamıyor | `app.UseCustomExceptionMiddleware()` eksik | `Program.cs`'e ekle |
| Tüm exception'lar 500 dönüyor | Abstract metod isimleri switch ile uyumsuz | `ExceptionHandler.cs` overload isimlerini kontrol et |
| ValidationContext\<object\> kullanımı | Type-safety kaybı | `ValidationContext<TRequest>` kullan |

---

## 10. İleri Seviye: Validator'ları Test Etmek

FluentValidation validator'ları bağımsız test edilebilir:

```csharp
public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public void Name_WhenEmpty_ShouldHaveValidationError()
    {
        var command = new CreateCustomerCommand { Name = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Müşteri adı boş olamaz.");
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new CreateCustomerCommand
        {
            Name = "Eczane A",
            LicenseNumber = "TR12345",
            Phone = "5551234567",
            Email = "info@eczane.com",
            Address = "İstanbul"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
```

---

## 11. Özet: Aklında Tutman Gereken 5 Şey

1. **FluentValidation = girdi kuralları**, BusinessRules = iş mantığı kuralları
2. **Her Command için bir Validator sınıfı** yaz, aynı klasöre koy
3. **`AddValidatorsFromAssembly`** tüm validator'ları otomatik kaydeder — manuel kayıt olmaz
4. **`RequestValidationBehavior`** handler çağrılmadan önce çalışır — MediatR pipeline
5. **`ExceptionMiddleware`** tüm exception'ları yakalar → RFC 7807 JSON döner

---

*Bu rehber PharmacyDepot projesi bağlamında yazılmıştır. Daha fazla öğrenmek için:*
- [FluentValidation Resmi Docs](https://docs.fluentvalidation.net)
- [MediatR Pipeline Behaviors](https://github.com/jbogard/MediatR/wiki/Behaviors)
- [RFC 7807 — Problem Details](https://tools.ietf.org/html/rfc7807)
