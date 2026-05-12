namespace CrossCuttingConcerns.Exceptions.Types;

// Bu dosya, istenen kaynağın bulunamadığı durumlarda fırlatılan özel exception türünü tanımlar.
// "Not Found Exception": Veritabanında aranılan kayıt (ilaç, müşteri, sipariş vb.) yoksa fırlatılır.
// HTTP 404 Not Found döndürülmesi için ExceptionHandler tarafından yakalanır.
// Örnek kullanım: throw new NotFoundException("Drug", id);

/// <summary>
/// İstenen kaynağın (entity) bulunamadığı durumlarda fırlatılan özel exception türü.
/// Yakalandığında HTTP 404 Not Found yanıtı üretilir.
/// </summary>
public class NotFoundException : Exception
{
    // Parametresiz constructor: bazı seri hale getirme senaryolarında kullanılır.
    public NotFoundException() { }

    // Yalnızca hata mesajı ile exception oluşturur.
    // Örnek: throw new NotFoundException("İlaç bulunamadı.");
    public NotFoundException(string? message) : base(message) { }

    // Entity adı ve ID ile okunabilir hata mesajı otomatik oluşturur.
    // Örnek: throw new NotFoundException("Drug", someGuid)
    // → "Drug with id '3fa85f64-...' not found."
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' not found.")
    {
    }
}
