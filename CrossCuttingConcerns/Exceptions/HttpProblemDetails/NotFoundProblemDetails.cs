using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrossCuttingConcerns.Exceptions.HttpProblemDetails;

// Bu dosya, "kayıt bulunamadı" hatalarını RFC 7807 Problem Details standardına uygun
// HTTP yanıt nesnesine dönüştürür.
// HTTP 404 Not Found: İstenen kaynağın sunucuda mevcut olmadığını bildirir.
// Örnek JSON yanıt:
// {
//   "title": "Not Found",
//   "detail": "Drug with id '3fa85f64-...' not found.",
//   "status": 404,
//   "type": "https://example.com/probs/not-found"
// }

/// <summary>
/// Kayıt bulunamadı hatalarını (NotFoundException) HTTP 404 yanıtına dönüştüren
/// ProblemDetails implementasyonu. RFC 7807 uyumludur.
/// </summary>
public class NotFoundProblemDetails : ProblemDetails
{
    /// <summary>
    /// "Kayıt bulunamadı" hatası için standart alanları doldurur.
    /// </summary>
    /// <param name="detail">Kullanıcıya gösterilecek açıklama (örn. "Drug with id '...' not found.").</param>
    public NotFoundProblemDetails(string detail)
    {
        // Hata başlığı
        Title = "Not Found";
        // NotFoundException mesajından gelen detay
        Detail = detail;
        // HTTP durum kodu: 404 Not Found
        Status = StatusCodes.Status404NotFound;
        // Hata türü için URI referansı
        Type = "https://example.com/probs/not-found";
    }
}
