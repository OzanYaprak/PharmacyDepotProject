using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrossCuttingConcerns.Exceptions.HttpProblemDetails;

// Bu dosya, iş kuralı ihlali hatalarını RFC 7807 Problem Details standardına uygun
// HTTP yanıt nesnesine dönüştürür.
// RFC 7807: API'lerin hata durumlarını tutarlı biçimde ifade etmesi için W3C standardı.
// Örnek JSON yanıt:
// {
//   "title": "Rule Violation",
//   "detail": "A drug with this GTIN already exists.",
//   "status": 400,
//   "type": "https://example.com/probs/business-rule-violation"
// }

/// <summary>
/// İş kuralı ihlallerini (BusinessException) HTTP 400 yanıtına dönüştüren ProblemDetails implementasyonu.
/// Microsoft.AspNetCore.Mvc.ProblemDetails sınıfından türer (RFC 7807 uyumlu).
/// </summary>
public class BusinessProblemDetails : ProblemDetails
{
    /// <summary>
    /// İş kuralı ihlali için standart alanları doldurur.
    /// </summary>
    /// <param name="detail">Kullanıcıya gösterilecek hata açıklaması (örn. "Bu GTIN zaten kayıtlı.").</param>
    public BusinessProblemDetails(string detail)
    {
        // Hatanın kısa başlığı (API tüketicisi ne tür hata olduğunu anlar)
        Title = "Rule Violation";
        // Hatanın detaylı açıklaması (BusinessException mesajından gelir)
        Detail = detail;
        // HTTP durum kodu: 400 Bad Request (istemci hatalı istek gönderdi)
        Status = StatusCodes.Status400BadRequest;
        // Hata tipi için URI referansı (RFC 7807 gereği; dokümantasyon URL'si olabilir)
        Type = "https://example.com/probs/business-rule-violation";
    }
}
