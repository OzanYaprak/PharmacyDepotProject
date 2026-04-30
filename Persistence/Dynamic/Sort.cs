namespace Persistence.Dynamic;

/// <summary>
/// Dinamik sorgularda kullanılan sıralama koşulunu temsil eder.
/// </summary>
public class Sort
{
    /// <summary>
    /// Varsayılan constructor; Field ve Direction alanlarını boş string ile başlatır.
    /// </summary>
    public Sort()
    {
        Field = string.Empty;
        Direction = string.Empty;
    }

    /// <summary>
    /// Sıralama nesnesini belirtilen alan adı ve yön ile oluşturur.
    /// </summary>
    /// <param name="field">Sıralama yapılacak alan adı (örn. "Name", "Price").</param>
    /// <param name="direction">Sıralama yönü (örn. "asc" veya "desc").</param>
    public Sort(string field, string direction)
    {
        Field = field;
        Direction = direction;
    }

    /// <summary>
    /// Sıralama yapılacak alanın adı (örn. "Name", "Price").
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Sıralama yönü. Artan sıra için "asc", azalan sıra için "desc" kullanılır.
    /// </summary>
    public string Direction { get; set; }
}