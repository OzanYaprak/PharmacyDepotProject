using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CrossCuttingConcerns.Exceptions.Extensions;

// Bu dosya, ProblemDetails nesnelerini JSON string'e dönüştürmek için
// extension method (genişletme metodu) sağlar.
// Extension method: var olan bir sınıfa o sınıfı değiştirmeden yeni metot eklemenin C# yoludur.
// Kullanım: new BusinessProblemDetails("Hata").AsJson()

/// <summary>
/// ProblemDetails nesneleri için yardımcı extension metodlar.
/// </summary>
public static class ProblemDetailsExtensions
{
    /// <summary>
    /// Herhangi bir ProblemDetails nesnesini JSON string'e dönüştürür.
    /// HttpExceptionHandler tarafından HTTP response body'ye yazılmak üzere kullanılır.
    /// Generic kısıt (where TProblemDetail : ProblemDetails) sayesinde yalnızca
    /// ProblemDetails türevleri için çalışır, tip güvenliği sağlar.
    /// </summary>
    /// <typeparam name="TProblemDetail">ProblemDetails'ten türeyen herhangi bir tür.</typeparam>
    /// <param name="problemDetails">JSON'a çevrilecek problem detayı nesnesi.</param>
    /// <returns>RFC 7807 uyumlu JSON string.</returns>
    public static string AsJson<TProblemDetail>(this TProblemDetail problemDetails) where TProblemDetail : ProblemDetails
    {
        // System.Text.Json ile serialization — Newtonsoft değil, .NET yerleşik kütüphane
        return JsonSerializer.Serialize(problemDetails);
    }
}
