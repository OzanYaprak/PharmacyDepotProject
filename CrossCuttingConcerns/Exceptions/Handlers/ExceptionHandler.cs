using CrossCuttingConcerns.Exceptions.Types;

namespace CrossCuttingConcerns.Exceptions.Handlers;

// Bu dosya, uygulama genelindeki exception yönetiminin çekirdeğini oluşturan
// soyut (abstract) ExceptionHandler sınıfını barındırır.
// Template Method tasarım deseni kullanılmıştır:
//   - HandleExceptionAsync: exception türüne göre doğru handler'ı seçer (public, çağrılabilir)
//   - HandleException(BusinessException): iş kuralı hatalarını işler (alt sınıf implement eder)
//   - HandleException(NotFoundException): kayıt bulunamadı hatalarını işler (alt sınıf implement eder)
//   - HandleException(Exception): beklenmeyen hataları işler (alt sınıf implement eder)

/// <summary>
/// Exception yönetiminin soyut temel sınıfı.
/// Alt sınıflar (örn. HttpExceptionHandler) bu sınıftan türeyerek
/// farklı ortamlara (HTTP, konsol, vb.) özgü hata işleme davranışı tanımlar.
/// Template Method deseni: HandleExceptionAsync ortak algoritma akışını yönetir.
/// </summary>
public abstract class ExceptionHandler
{
    /// <summary>
    /// Gelen exception'ın türüne göre uygun handler metodunu çağırır.
    /// C# switch expression kullanılarak tür eşleştirmesi (pattern matching) yapılır.
    /// BusinessException → iş kuralı handler'ına,
    /// NotFoundException → kayıt bulunamadı handler'ına,
    /// diğer tüm türler → genel (500) handler'a yönlendirilir.
    /// </summary>
    public Task HandleExceptionAsync(Exception exception) =>
        exception switch
        {
            // exception BusinessException türündeyse iş kuralı handler'ını çağır
            BusinessException businessException => HandleException(businessException),
            // exception NotFoundException türündeyse 404 handler'ını çağır
            NotFoundException notFoundException => HandleException(notFoundException),
            // Diğer tüm exception türleri için genel (500) handler'ı çağır
            _ => HandleException(exception)
        };

    // Alt sınıfın implement etmesi zorunlu: iş kuralı hata işleme davranışını tanımlar
    protected abstract Task BusinessHandleException(BusinessException businessException);

    // Alt sınıfın implement etmesi zorunlu: kayıt bulunamadı hata işleme davranışını tanımlar
    protected abstract Task NotFoundHandleException(NotFoundException notFoundException);

    // Alt sınıfın implement etmesi zorunlu: beklenmeyen hata işleme davranışını tanımlar
    protected abstract Task HandleException(Exception exception);
}
