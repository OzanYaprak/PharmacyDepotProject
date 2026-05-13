using CrossCuttingConcerns.Exceptions.Extensions;
using CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using CrossCuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CrossCuttingConcerns.Exceptions.Handlers;

// Bu dosya, ExceptionHandler soyut sınıfının HTTP ortamına özgü implementasyonunu barındırır.
// Görev: exception'ları HTTP yanıtına (response) dönüştürmek.
// - BusinessException  → HTTP 400 Bad Request + BusinessProblemDetails JSON
// - NotFoundException  → HTTP 404 Not Found + NotFoundProblemDetails JSON
// - Exception (diğer) → HTTP 500 Internal Server Error + InternalServerErrorProblemDetails JSON

/// <summary>
/// HTTP isteklerinde oluşan exception'ları HTTP yanıtına dönüştüren handler.
/// ExceptionMiddleware tarafından kullanılır.
/// RFC 7807 Problem Details standardına uygun JSON yanıtlar döndürür.
/// </summary>
public class HttpExceptionHandler : ExceptionHandler
{
    // Nullable HttpResponse alanı: kullanılmadan önce set edilmesi zorunludur.
    // Middleware her istek için bu property'yi doldurur.
    private HttpResponse? _httpResponse;

    /// <summary>
    /// İşlenecek HTTP yanıtı. Kullanılmadan önce set edilmezse ArgumentNullException fırlatır.
    /// Bu tasarım, handler'ın yanlışlıkla Response olmadan çalışmasını engeller.
    /// </summary>
    public HttpResponse Response 
    { 
        get => _httpResponse ?? throw new ArgumentNullException(nameof(_httpResponse));
        set => _httpResponse = value;
    }

    /// <summary>
    /// İş kuralı ihlali (BusinessException) için HTTP 400 yanıtı oluşturur.
    /// Kullanıcıya anlamlı hata mesajı döndürülür.
    /// </summary>
    protected override Task BusinessHandleException(BusinessException businessException)
    {
        // HTTP durum kodu 400 olarak ayarlanır: istemci hatası (kullanıcı hatalı veri gönderdi)
        Response.StatusCode = StatusCodes.Status400BadRequest;
        // BusinessProblemDetails nesnesi JSON'a dönüştürülüp response body'ye yazılır
        string details = new BusinessProblemDetails(businessException.Message).AsJson();
        return Response.WriteAsync(details);
    }

    /// <summary>
    /// Kayıt bulunamadı (NotFoundException) için HTTP 404 yanıtı oluşturur.
    /// </summary>
    protected override Task NotFoundHandleException(NotFoundException notFoundException)
    {
        // HTTP durum kodu 404 olarak ayarlanır: istenen kaynak sunucuda yok
        Response.StatusCode = StatusCodes.Status404NotFound;
        // NotFoundProblemDetails nesnesi JSON'a dönüştürülüp response body'ye yazılır
        string details = new NotFoundProblemDetails(notFoundException.Message).AsJson();
        return Response.WriteAsync(details);
    }

    /// <summary>
    /// Beklenmeyen (sunucu kaynaklı) hatalar için HTTP 500 yanıtı oluşturur.
    /// Hassas hata detayları production ortamında gizlenmelidir.
    /// </summary>
    protected override Task HandleException(Exception exception)
    {
        // HTTP durum kodu 500 olarak ayarlanır: sunucu hatası (kodda beklenmeyen hata oluştu)
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        // InternalServerErrorProblemDetails: 500 hatası için standart RFC 7807 yanıtı
        string details = new InternalServerErrorProblemDetails(exception.Message).AsJson();
        return Response.WriteAsync(details);
    }
}