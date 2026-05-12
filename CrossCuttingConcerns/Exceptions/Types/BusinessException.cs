namespace CrossCuttingConcerns.Exceptions.Types;

// Bu dosya, uygulamaya özgü "iş kuralı ihlali" hatalarını temsil eden exception sınıfını barındırır.
// "Business Exception" (İş Kuralı İstisnası): Kullanıcının hatalı veri girmesi,
// aynı GTIN ile kayıt oluşturmaya çalışması gibi durumlarda fırlatılır.
// HTTP 400 Bad Request döndürülmesi için ExceptionHandler tarafından yakalanır.

/// <summary>
/// Uygulamanın iş kurallarının ihlal edildiği durumlarda fırlatılan özel exception türü.
/// Örnek: Aynı GTIN'e sahip iki ilaç oluşturulmaya çalışılması.
/// Bu exception yakalandığında HTTP 400 Bad Request döndürülür.
/// </summary>
public class BusinessException : Exception
{
    // Parametresiz constructor: bazı seri hale getirme senaryolarında veya
    // mesajsız fırlatma gereken durumlarda kullanılır.
    public BusinessException() { }

    // Yalnızca hata mesajı ile exception oluşturur.
    // En sık kullanılan yol: throw new BusinessException("Hata mesajı");
    public BusinessException(string? message) : base(message) { }

    // Hem hata mesajı hem de iç exception ile oluşturur.
    // Başka bir exception'ı sarmalamak (wrap) için kullanılır.
    public BusinessException(string? message, Exception? innerException) : base(message, innerException) { }
}
