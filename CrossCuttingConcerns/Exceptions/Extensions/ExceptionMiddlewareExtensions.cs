using CrossCuttingConcerns.Exceptions.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace CrossCuttingConcerns.Exceptions.Extensions;

// Bu dosya, ExceptionMiddleware'i ASP.NET Core pipeline'ına eklemek için
// IApplicationBuilder üzerinde bir extension method tanımlar.
// Kullanım (Program.cs'te): app.UseCustomExceptionMiddleware();
// Bu sayede middleware kayıt kodu Program.cs'te okunabilir ve kısa kalır.

/// <summary>
/// IApplicationBuilder için ExceptionMiddleware kayıt extension metodunu içerir.
/// Clean Architecture prensibine uygun olarak middleware kaydı bu katmanda kapsüllenir.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Global exception middleware'ini HTTP pipeline'ına ekler.
    /// Program.cs'te tüm diğer middleware'lerden ÖNCE çağrılmalıdır ki
    /// hiçbir exception gözden kaçmasın.
    /// </summary>
    /// <param name="app">ASP.NET Core uygulama pipeline builder'ı.</param>
    public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        // UseMiddleware<T>: ExceptionMiddleware'i pipeline'a sıradaki adım olarak kaydeder.
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
