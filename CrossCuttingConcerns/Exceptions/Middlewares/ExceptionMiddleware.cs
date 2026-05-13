using CrossCuttingConcerns.Exceptions.Handlers;
using Microsoft.AspNetCore.Http;

namespace CrossCuttingConcerns.Exceptions.Middlewares;

// Bu dosya, ASP.NET Core middleware pipeline'ındaki global hata yakalama katmanını barındırır.
// Middleware: her HTTP isteğinin geçtiği işlem adımlarından biridir (pipeline).
// ExceptionMiddleware, tüm controller ve servislerden fırlayan exception'ları tek merkezde yakalar.
// Böylece her endpoint'te try/catch yazmaya gerek kalmaz.

/// <summary>
/// Uygulama genelinde fırlatılan tüm exception'ları yakalayan ve
/// uygun HTTP yanıtına dönüştüren ASP.NET Core middleware'i.
/// Program.cs'te app.UseCustomExceptionMiddleware() ile pipeline'a eklenir.
/// </summary>
public class ExceptionMiddleware
{
    #region Constructor and Fields

    // _next: pipeline'daki bir sonraki middleware'i temsil eder.
    // Middleware zincirinde sıradaki halka çağrılmazsa istek ilerlemez.
    private readonly RequestDelegate _next;

    // _httpExceptionHandler: exception'ı HTTP yanıtına dönüştürmekten sorumlu nesne.
    private readonly HttpExceptionHandler _httpExceptionHandler;

    /// <summary>
    /// ASP.NET Core DI container tarafından otomatik çağrılır.
    /// </summary>
    /// <param name="next">Pipeline'daki sonraki middleware delegesi.</param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
        // HttpExceptionHandler burada bir kez oluşturulur; her istek için Response property'si güncellenir.
        _httpExceptionHandler = new HttpExceptionHandler();
    }

    #endregion

    /// <summary>
    /// Her HTTP isteğinde çalışan ana metot.
    /// İsteği sonraki middleware'e iletir; exception fırlarsa yakalar ve işler.
    /// </summary>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            // Bir sonraki middleware'i (veya controller'ı) çağır.
            // Hata yoksa istek buradan devam eder, exception bloğu atlanır.
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            // Pipeline'ın herhangi bir yerinde exception fırlarsa burası devreye girer.
            // httpContext.Response: istemciye gönderilecek HTTP yanıtı nesnesi.
            await HandleExceptionAsync(httpContext.Response, exception);
        }
    }

    /// <summary>
    /// Response nesnesini hazırlar ve exception handler'a iletir.
    /// </summary>
    private Task HandleExceptionAsync(HttpResponse httpResponse, Exception exception)
    {
        // Yanıtın içerik türü JSON olarak ayarlanır (RFC 7807 gereği)
        httpResponse.ContentType = "application/json";
        // Handler'a hangi response'a yazacağını söyler
        _httpExceptionHandler.Response = httpResponse;
        // Exception türüne göre (Business/Generic) doğru handler metodunu çağırır
        return _httpExceptionHandler.HandleExceptionAsync(exception);
    }
}