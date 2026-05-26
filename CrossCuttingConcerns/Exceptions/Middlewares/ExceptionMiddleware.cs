using CrossCuttingConcerns.Exceptions.Handlers;
using CrossCuttingConcerns.Exceptions.Helpers;
using CrossCuttingConcerns.Logging;
using CrossCuttingConcerns.Serilog;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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

    // _httpContextAccessor: HttpContext'a erişim sağlayan nesne (gerekirse).
    private readonly IHttpContextAccessor _httpContextAccessor;

    // _loggerServiceBase: exception'ları loglamak için kullanılan logger nesnesi.
    private readonly LoggerServiceBase _loggerService;

    /// <summary>
    /// ASP.NET Core DI container tarafından otomatik çağrılır.
    /// </summary>
    /// <param name="next">Pipeline'daki sonraki middleware delegesi.</param>
    /// <param name="httpContextAccessor">HttpContext'e erişim sağlayan nesne.</param>
    /// <param name="loggerService">Exception'ları loglamak için kullanılan logger nesnesi.</param>
    public ExceptionMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, LoggerServiceBase loggerService)
    {
        _next = next;
        _httpContextAccessor = httpContextAccessor;
        _loggerService = loggerService;
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
            // Exception yakalandığında önce loglanır, sonra uygun HTTP yanıtı hazırlanır.
            // Loglama işlemi senkron olarak yapılır (LogException Task.CompletedTask döndürüyor).
            // Loglama, exception'ın detaylarını (mesaj, stack trace, vb.) içerir.
            await LogException(httpContext, exception);

            // Pipeline'ın herhangi bir yerinde exception fırlarsa burası devreye girer.
            // httpContext.Response: istemciye gönderilecek HTTP yanıtı nesnesi.
            await HandleExceptionAsync(httpContext.Response, exception);
        }
    }

    /// <summary>
    /// Yapılandırılmış logger servisi aracılığıyla exception detaylarını loglar.
    /// </summary>
    /// <param name="httpContext">Mevcut HTTP bağlamı.</param>
    /// <param name="exception">Loglanacak exception.</param>
    /// <returns>Asenkron işlemi temsil eden görev nesnesi.</returns>
    private Task LogException(HttpContext httpContext, Exception exception)
    {
        List<LogParameter> logParameters = new List<LogParameter>()
        {
            new LogParameter
            {
                Name = exception.GetType().Name,
                Type = exception.GetType().Name,
                Value = exception.GetType().FullName ?? exception.GetType().Name 
            }
        };

        var user = httpContext?.User?.Identity?.Name;
        var requestPath = httpContext?.Request?.Path.Value ?? string.Empty;
        var requestMethod = httpContext?.Request?.Method ?? string.Empty;

        LogDetailWithException logDetail = new LogDetailWithException
        {
            Fullname = exception.GetType().FullName ?? exception.GetType().Name,
            MethodName = $"[{requestMethod}] {requestPath} - Exception",
            Parameters = logParameters,
            User = string.IsNullOrWhiteSpace(user) ? "Anonymous" : user!, 
            ExceptionMessage = CharacterTransliteration.TransliterateToEnglish(exception.Message)
        };

        _loggerService.Error(JsonSerializer.Serialize(logDetail));

        return Task.CompletedTask;
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