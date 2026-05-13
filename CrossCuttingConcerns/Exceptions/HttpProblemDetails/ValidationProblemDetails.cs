using CrossCuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrossCuttingConcerns.Exceptions.HttpProblemDetails;

// Bu dosya, validation hatalarının istemciye döndürülmesi için kullanılan
// RFC 7807 uyumlu HTTP yanıt modelini barındırır.
//
// 📋 RFC 7807 — "Problem Details for HTTP APIs":
//   HTTP hata yanıtları için resmi standart. Temel alanlar:
//     - title  : hatanın kısa başlığı (örn. "Validation Failed")
//     - detail : hatanın açıklaması (örn. "One or more validation errors occurred.")
//     - status : HTTP durum kodu (örn. 400)
//     - type   : hatayı tanımlayan URI referansı
//
// ValidationProblemDetails, standart ProblemDetails'e EK OLARAK "errors" listesi ekler.
// Bu sayede istemci (frontend) her alan için hangi hataların oluştuğunu bilir.
//
// Örnek JSON yanıt:
// {
//   "title": "Validation Failed",
//   "detail": "One or more validation errors occurred.",
//   "status": 400,
//   "type": "https://example.com/probs/validation-error",
//   "errors": [
//     { "propertyName": "Email", "errorMessages": ["Boş olamaz", "Geçersiz format"] },
//     { "propertyName": "Name",  "errorMessages": ["Boş olamaz"] }
//   ]
// }

/// <summary>
/// Doğrulama hataları için RFC 7807 uyumlu HTTP yanıt modeli.
/// ASP.NET Core'un yerleşik <see cref="ProblemDetails"/> sınıfından türetilmiştir.
/// Standart alanlara ek olarak alan bazlı hata listesi (<see cref="Errors"/>) içerir.
/// </summary>
public class ValidationProblemDetails : ProblemDetails
{
    /// <summary>
    /// Alan bazlı doğrulama hatalarının listesi.
    /// JSON yanıtta "errors" anahtarı olarak serileştirilir.
    /// init: nesne oluşturulduktan sonra değiştirilemez (immutability).
    /// </summary>
    public IEnumerable<ValidationExceptionModel>? Errors { get; init; } = default!;

    /// <summary>
    /// Hata listesiyle birlikte RFC 7807 uyumlu yanıt modeli oluşturur.
    /// </summary>
    /// <param name="errors">Alan bazlı doğrulama hataları.</param>
    public ValidationProblemDetails(IEnumerable<ValidationExceptionModel>? errors)
    {
        Title = "Validation Failed";
        Detail = "One or more validation errors occurred.";
        Errors = errors;
        Status = StatusCodes.Status400BadRequest;
        // Type: hata türünü tanımlayan URI. Production'da gerçek bir dokümantasyon URL'siyle değiştirilmeli.
        Type = "https://example.com/probs/validation-error";
    }
}
