namespace Persistence.Dynamic;

/// <summary>
/// Dinamik sorgularda kullanılan sıralama ve filtreleme koşullarını bir arada tutan sorgu nesnesidir.
/// </summary>
public class DynamicQuery
{
    /// <summary>
    /// Sorguya uygulanacak sıralama koşullarının listesi. Belirtilmezse sıralama yapılmaz.
    /// </summary>
    public IEnumerable<Sort>? Sort { get; set; }

    /// <summary>
    /// Sorguya uygulanacak filtre koşulu. Belirtilmezse filtreleme yapılmaz.
    /// </summary>
    public Filter? Filter { get; set; }

    /// <summary>
    /// Varsayılan constructor; sıralama ve filtreleme koşulları sonradan atanabilir.
    /// </summary>
    public DynamicQuery() { }

    /// <summary>
    /// Dinamik sorgu nesnesini belirtilen sıralama ve filtre koşullarıyla oluşturur.
    /// </summary>
    /// <param name="sort">Uygulanacak sıralama koşullarının listesi.</param>
    /// <param name="filter">Uygulanacak filtre koşulu.</param>
    public DynamicQuery(IEnumerable<Sort>? sort, Filter? filter)
    {
        Sort = sort;
        Filter = filter;
    }
}
