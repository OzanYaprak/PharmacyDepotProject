using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrossCuttingConcerns.Exceptions.HttpProblemDetails;

// Bu dosya, beklenmeyen sunucu hatalarını RFC 7807 Problem Details standardına uygun
// HTTP yanıt nesnesine dönüştürür.
// HTTP 500 Internal Server Error: Sunucu tarafında öngörülemeyen bir hata oluştuğunu belirtir.
// Örnek JSON yanıt:
// {
//   "title": "Internal Server Error",
//   "detail": "Object reference not set to an instance of an object.",
//   "status": 500,
//   "type": "https://example.com/probs/internal-server-error"
// }

/// <summary>
/// Beklenmeyen (sunucu kaynaklı) hataları HTTP 500 yanıtına dönüştüren ProblemDetails implementasyonu.
/// Microsoft.AspNetCore.Mvc.ProblemDetails sınıfından türer (RFC 7807 uyumlu).
/// </summary>
public class InternalServerErrorProblemDetails : ProblemDetails
{
    /// <summary>
    /// Sunucu hatası için standart alanları doldurur.
    /// </summary>
    /// <param name="detail">Hata açıklaması. Production'da bu detay gizlenmelidir (stack trace içerebilir).</param>
    public InternalServerErrorProblemDetails(string detail)
    {
        // Hatanın kısa başlığı
        Title = "Internal Server Error";
        // Exception'ın mesajı buraya yazılır — dikkat: prod ortamında hassas bilgi içerebilir
        Detail = detail;
        // HTTP durum kodu: 500 (sunucu hatası)
        Status = StatusCodes.Status500InternalServerError;
        // Hata tipi için URI referansı
        Type = "https://example.com/probs/internal-server-error";
    }
}