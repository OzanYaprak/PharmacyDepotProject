namespace Persistence.Dynamic;

/// <summary>
/// Dinamik sorgularda kullanılan filtre koşulunu temsil eder.
/// </summary>
public class Filter
{
    /// <summary>
    /// Varsayılan constructor; Field ve Operator alanlarını boş string ile başlatır.
    /// </summary>
    public Filter()
    {
        Field = string.Empty;
        Operator = string.Empty;
    }

    /// <summary>
    /// Filtre nesnesini belirtilen alan adı ve operatör ile oluşturur.
    /// </summary>
    /// <param name="field">Filtreleme yapılacak alan adı.</param>
    /// <param name="operator">Karşılaştırma operatörü (örn. "eq", "contains", "gt").</param>
    public Filter(string field, string @operator)
    {
        Field = field;
        Operator = @operator;
    }

    /// <summary>
    /// Filtreleme yapılacak alanın adı (örn. "Name", "Price").
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Filtreleme değeri. Null olabilir.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Karşılaştırma operatörü (örn. "eq", "neq", "lt", "gt", "contains").
    /// </summary>
    public string Operator { get; set; }

    /// <summary>
    /// Birden fazla filtre varsa aralarındaki mantıksal bağlaç (örn. "and", "or"). Null olabilir.
    /// </summary>
    public string? Logic { get; set; }

    /// <summary>
    /// İç içe (nested) filtre koşulları. Karmaşık sorgular için kullanılır. Null olabilir.
    /// </summary>
    public IEnumerable<Filter>? Filters { get; set; }
}
