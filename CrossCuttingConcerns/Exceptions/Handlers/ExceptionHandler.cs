using CrossCuttingConcerns.Exceptions.Types;

namespace CrossCuttingConcerns.Exceptions.Handlers;

// Bu dosya, uygulama genelindeki exception yönetiminin çekirdeğini oluşturan
// soyut (abstract) ExceptionHandler sınıfını barındırır.
//
// 📐 Template Method Tasarım Deseni (Design Pattern):
//   Bu desen şu şekilde çalışır:
//   - Üst sınıf (ExceptionHandler) "algoritmanın iskeletini" tanımlar → HandleExceptionAsync
//   - Alt sınıflar (HttpExceptionHandler) her adımın "nasıl yapılacağını" belirler → override metodlar
//
// Metot rolleri:
//   - HandleExceptionAsync : exception türüne göre doğru handler'ı seçen yönlendirici (public)
//   - HandleException(BusinessException)  : iş kuralı hataları    → HTTP 400
//   - HandleException(NotFoundException)  : kayıt bulunamadı      → HTTP 404
//   - HandleException(ValidationException): doğrulama hataları    → HTTP 400 (ayrı format)
//   - HandleException(Exception)          : beklenmeyen hatalar   → HTTP 500

/// <summary>
/// Exception yönetiminin soyut temel sınıfı.
/// Alt sınıflar (örn. HttpExceptionHandler) bu sınıftan türeyerek
/// farklı ortamlara (HTTP, konsol, test, vb.) özgü hata işleme davranışı tanımlar.
/// Template Method deseni: HandleExceptionAsync ortak algoritma akışını yönetir,
/// alt sınıflar her exception türü için somut davranışı override eder.
/// </summary>
public abstract class ExceptionHandler
{
    /// <summary>
    /// Gelen exception'ın türüne göre uygun handler metodunu çağırır.
    /// C# switch expression + pattern matching kullanılmıştır:
    ///   exception switch { TypeA a => ..., TypeB b => ..., _ => ... }
    ///
    /// ÖNEMLİ: Sıralama kritiktir! Daha spesifik türler daha önce yazılmalıdır.
    /// Örneğin BusinessException, Exception'dan önce yazılmazsa _ kolu hepsini yakalar.
    /// </summary>
    public Task HandleExceptionAsync(Exception exception) =>
        exception switch
        {
            // exception BusinessException türündeyse iş kuralı handler'ını çağır (HTTP 400)
            BusinessException businessException => HandleException(businessException),
            // exception NotFoundException türündeyse kayıt bulunamadı handler'ını çağır (HTTP 404)
            NotFoundException notFoundException => HandleException(notFoundException),
            // exception ValidationException türündeyse doğrulama hatası handler'ını çağır (HTTP 400)
            ValidationException validationException => HandleException(validationException),
            // Yukarıdaki hiçbir türe uymayan tüm exception'lar için genel (HTTP 500) handler
            _ => HandleException(exception)
        };

    // Alt sınıfın implement etmesi zorunlu: iş kuralı hata işleme davranışını tanımlar (HTTP 400)
    protected abstract Task HandleException(BusinessException businessException);

    // Alt sınıfın implement etmesi zorunlu: kayıt bulunamadı hata işleme davranışını tanımlar (HTTP 404)
    protected abstract Task HandleException(NotFoundException notFoundException);

    // Alt sınıfın implement etmesi zorunlu: doğrulama hatası işleme davranışını tanımlar (HTTP 400)
    protected abstract Task HandleException(ValidationException validationException);

    // Alt sınıfın implement etmesi zorunlu: beklenmeyen/sunucu kaynaklı hata işleme davranışını tanımlar (HTTP 500)
    protected abstract Task HandleException(Exception exception);
}
