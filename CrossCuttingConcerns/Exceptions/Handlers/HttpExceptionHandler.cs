using CrossCuttingConcerns.Exceptions.Extensions;
using CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using CrossCuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;

namespace CrossCuttingConcerns.Exceptions.Handlers;

// Bu dosya, ExceptionHandler soyut sınıfının HTTP ortamına özgü implementasyonunu barındırır.
//
// Görev: fırlatılan exception'ları istemciye dönecek HTTP yanıtına dönüştürmek.
// Her exception türü için farklı bir HTTP durum kodu ve JSON yanıt formatı kullanılır:
//
//   BusinessException   → HTTP 400 Bad Request  + BusinessProblemDetails JSON
//   NotFoundException   → HTTP 404 Not Found    + NotFoundProblemDetails JSON
//   ValidationException → HTTP 400 Bad Request  + ValidationProblemDetails JSON (hata listesi içerir)
//   Exception (diğer)   → HTTP 500 Server Error + InternalServerErrorProblemDetails JSON
//
// RFC 7807: "Problem Details for HTTP APIs" — HTTP hata yanıtları için resmi standart.
// Bu standart, title/detail/status/type alanlarını içeren JSON formatını tanımlar.

/// <summary>
/// HTTP isteklerinde oluşan exception'ları HTTP yanıtına dönüştüren somut handler.
/// ExceptionMiddleware tarafından her istekte kullanılır.
/// RFC 7807 Problem Details standardına uygun JSON yanıtlar döndürür.
/// </summary>
public class HttpExceptionHandler : ExceptionHandler
{
    // Nullable HttpResponse alanı: kullanılmadan önce set edilmesi zorunludur.
    // Middleware, her HTTP isteği için bu property'yi doldurur.
    // Böylece tek bir HttpExceptionHandler nesnesi birden fazla istek için yeniden kullanılabilir.
    private HttpResponse? _httpResponse;

    /// <summary>
    /// İşlenecek HTTP yanıtı. Set edilmeden erişilirse ArgumentNullException fırlatır.
    /// Bu tasarım, handler'ın Response olmadan yanlışlıkla çalışmasını önler (fail-fast prensibi).
    /// </summary>
    public HttpResponse Response
    {
        get => _httpResponse ?? throw new ArgumentNullException(nameof(_httpResponse));
        set => _httpResponse = value;
    }

    /// <summary>
    /// İş kuralı ihlali (BusinessException) için HTTP 400 yanıtı oluşturur.
    /// Örnek: "Bu ilaç zaten kayıtlı." gibi domain seviyesi ihlaller buraya düşer.
    /// </summary>
    protected override Task HandleException(BusinessException businessException)
    {
        // HTTP 400: istemcinin gönderdiği veri/talep iş kurallarına aykırı
        Response.StatusCode = StatusCodes.Status400BadRequest;
        // BusinessProblemDetails → RFC 7807 uyumlu JSON yanıt
        string details = new BusinessProblemDetails(businessException.Message).AsJson();
        return Response.WriteAsync(details);
    }

    /// <summary>
    /// Kayıt bulunamadı (NotFoundException) için HTTP 404 yanıtı oluşturur.
    /// Örnek: "Id=5 olan ilaç bulunamadı." gibi durumlar buraya düşer.
    /// </summary>
    protected override Task HandleException(NotFoundException notFoundException)
    {
        // HTTP 404: istenen kaynak sunucuda mevcut değil
        Response.StatusCode = StatusCodes.Status404NotFound;
        // NotFoundProblemDetails → RFC 7807 uyumlu JSON yanıt
        string details = new NotFoundProblemDetails(notFoundException.Message).AsJson();
        return Response.WriteAsync(details);
    }

    /// <summary>
    /// Doğrulama hataları (ValidationException) için HTTP 400 yanıtı oluşturur.
    /// BusinessException'dan farklı olarak birden fazla hata içerebilir (Errors listesi).
    /// Örnek: Name boş, Email geçersiz gibi birden fazla alan hatası aynı anda döner.
    /// </summary>
    protected override Task HandleException(ValidationException validationException)
    {
        // HTTP 400: istemci geçersiz/eksik veri gönderdi
        Response.StatusCode = StatusCodes.Status400BadRequest;
        // ValidationProblemDetails → hata listesini (Errors) de içeren özel RFC 7807 yanıtı
        string details = new ValidationProblemDetails(validationException.Errors).AsJson();
        return Response.WriteAsync(details);
    }

    /// <summary>
    /// Beklenmeyen (sunucu kaynaklı) hatalar için HTTP 500 yanıtı oluşturur.
    /// Production ortamında exception.Message yerine genel bir mesaj döndürülmesi önerilir.
    /// </summary>
    protected override Task HandleException(Exception exception)
    {
        // HTTP 500: sunucu tarafında beklenmeyen bir hata oluştu
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        // InternalServerErrorProblemDetails → RFC 7807 uyumlu 500 yanıt formatı
        string details = new InternalServerErrorProblemDetails(exception.Message).AsJson();
        return Response.WriteAsync(details);
    }
}