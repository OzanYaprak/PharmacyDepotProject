namespace CrossCuttingConcerns.Exceptions.Types;

// Bu dosya, doğrulama hatalarını taşıyan özel exception sınıfını barındırır.
//
// 🎯 Neden Özel Exception Sınıfı?
//   .NET'in yerleşik Exception sınıfı yalnızca tek bir hata mesajı (string Message) taşır.
//   Oysa bir form gönderildiğinde birden fazla alan hatalı olabilir:
//     - Name boş
//     - Email geçersiz
//     - Phone eksik
//   Bu exception, tüm bu hataları tek seferde yapılandırılmış şekilde taşır.
//
// 🏗️ Mimari:
//   RequestValidationBehavior → ValidationException fırlatır
//   ExceptionMiddleware       → yakalar
//   HttpExceptionHandler      → HTTP 400 + ValidationProblemDetails JSON döndürür

/// <summary>
/// FluentValidation doğrulama hatalarını birden fazla alan bazında taşıyan özel exception.
/// <see cref="ValidationExceptionModel"/> listesi aracılığıyla hangi alanın
/// hangi hatalar içerdiğini yapılandırılmış biçimde sunar.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Alan bazlı doğrulama hataları listesi.
    /// Her eleman: bir alan adı (PropertyName) ve o alana ait hata mesajları (ErrorMessages).
    /// Örnek: { PropertyName = "Email", ErrorMessages = ["Boş olamaz", "Geçersiz format"] }
    /// </summary>
    public IEnumerable<ValidationExceptionModel>? Errors { get; set; } = default!;

    /// <summary>
    /// Parametresiz constructor: boş hata listesiyle exception oluşturur.
    /// Genellikle hata detayları sonradan atanacaksa kullanılır.
    /// </summary>
    public ValidationException() : base()
    {
        Errors = Array.Empty<ValidationExceptionModel>();
    }

    /// <summary>
    /// Tek mesajlı constructor: standart Exception mesajı ile oluşturur.
    /// Hata listesi boş kalır; yalnızca genel bir mesaj taşır.
    /// </summary>
    public ValidationException(string? message) : base(message)
    {
        Errors = Array.Empty<ValidationExceptionModel>();
    }

    /// <summary>
    /// İç exception (innerException) ile oluşturur.
    /// Başka bir exception'ı sarmalamanız gerektiğinde kullanılır.
    /// </summary>
    public ValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
        Errors = Array.Empty<ValidationExceptionModel>();
    }

    /// <summary>
    /// Asıl kullanım: alan bazlı hata listesiyle exception oluşturur.
    /// RequestValidationBehavior bu constructor'ı kullanır.
    /// Hata mesajı <see cref="BuildErrorMessage"/> ile otomatik oluşturulur.
    /// </summary>
    public ValidationException(IEnumerable<ValidationExceptionModel> errors) : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    /// <summary>
    /// Hata listesinden okunabilir bir mesaj metni üretir.
    /// Bu metin Exception.Message property'sine atanır ve loglarda görünür.
    /// Örnek: "Validation failed for the following properties: Name: Boş olamaz, Email: Geçersiz format"
    /// </summary>
    private static string BuildErrorMessage(IEnumerable<ValidationExceptionModel> errors)
    {
        // Her hata modeli için "AlanAdı: Hata1, Hata2" formatında satır oluştur
        IEnumerable<string> errorMessages = errors.Select(e =>
            $"{e.PropertyName}: {string.Join(", ", e.ErrorMessages ?? Array.Empty<string>())}");

        return $"Validation failed for the following properties: {string.Join(string.Empty, errorMessages)}";
    }
}

/// <summary>
/// Tek bir alana (property) ait doğrulama hata modelini temsil eder.
/// <see cref="ValidationException.Errors"/> listesinin her elemanı bu türdendir.
/// </summary>
public class ValidationExceptionModel
{
    /// <summary>
    /// Hatalı alanın adı. FluentValidation'ın failure.PropertyName değerinden gelir.
    /// Örnek: "Name", "Email", "Phone"
    /// </summary>
    public string PropertyName { get; set; } = default!;

    /// <summary>
    /// Bu alana ait hata mesajları listesi.
    /// Bir alan birden fazla kuralı ihlal edebilir; bu durumda birden fazla mesaj içerir.
    /// Örnek: ["Boş olamaz", "100 karakterden uzun olamaz"]
    /// </summary>
    public IEnumerable<string>? ErrorMessages { get; set; } = default!;
}
