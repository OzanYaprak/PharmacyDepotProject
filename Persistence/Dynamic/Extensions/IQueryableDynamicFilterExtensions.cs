using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Persistence.Dynamic.Extensions;

/// <summary>
/// IQueryable için dinamik filtreleme ve sıralama uzantı metodlarını sağlar.
/// </summary>
public static class IQueryableDynamicFilterExtensions
{
    // Geçerli sıralama yönleri: artan (asc) ve azalan (desc)
    private static readonly string[] _orders = { "asc", "desc" };

    // Birden fazla filtre koşulunu birleştirmek için geçerli mantıksal operatörler
    private static readonly string[] _logics = { "and", "or" };

    // Desteklenen filtre operatörlerinin LINQ Dynamic Core karşılıklarına eşlemesi
    private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
    {
        { "eq", "=" },              // Eşit
        { "neq", "!=" },            // Eşit değil
        { "lt", "<" },              // Küçük
        { "lte", "<=" },            // Küçük veya eşit
        { "gt", ">" },              // Büyük
        { "gte", ">=" },            // Büyük veya eşit
        { "isnull", "== null" },    // Null kontrolü
        { "isnotnull", "!= null" }, // Null olmama kontrolü
        { "startswith", "StartsWith" },     // Belirtilen değerle başlar
        { "endswith", "EndsWith" },         // Belirtilen değerle biter
        { "contains", "Contains" },         // Belirtilen değeri içerir
        { "doesnotcontain", "Contains" }    // Belirtilen değeri içermez (olumsuz Contains)
    };

    /// <summary>
    /// Verilen <see cref="DynamicQuery"/> nesnesine göre sorguya dinamik filtreleme ve sıralama uygular.
    /// </summary>
    /// <typeparam name="T">Sorgu öğesinin tipi.</typeparam>
    /// <param name="query">Uygulanacak kaynak sorgu.</param>
    /// <param name="dynamicQuery">Filtre ve sıralama bilgilerini içeren dinamik sorgu nesnesi.</param>
    /// <returns>Filtre ve sıralama uygulanmış <see cref="IQueryable{T}"/>.</returns>
    public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery dynamicQuery)
    {
        // Filtre varsa sorguya uygula
        if (dynamicQuery.Filter is not null)
        {
            query = Filter(query, dynamicQuery.Filter);
        }

        // Sıralama kriteri varsa sorguya uygula
        if (dynamicQuery.Sort is not null && dynamicQuery.Sort.Any())
        {
            query = Sort(query, dynamicQuery.Sort);
        }

        return query;
    }

    #region Helper Methods

    /// <summary>
    /// Verilen filtre nesnesini sorguya uygular.
    /// </summary>
    private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
    {
        // Tüm iç içe filtreleri düz liste olarak topla
        IList<Filter> filters = GetAllFilters(filter);

        // Her filtrenin değerini parametre dizisi olarak hazırla
        string?[] values = filters.Select(f => f.Value).ToArray();

        // Filtre koşulunu LINQ Dynamic Core formatına dönüştür
        string where = Transform(filter, filters);

        if (!string.IsNullOrEmpty(where) && values != null)
        {
            queryable = queryable.Where(where, values);
        }

        return queryable;
    }

    /// <summary>
    /// Verilen sıralama kriterlerini sorguya uygular.
    /// </summary>
    private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
    {
        // Her sıralama kriterinin alan adı ve yönünü doğrula
        foreach (Sort item in sort)
        {
            if (string.IsNullOrEmpty(item.Field)) { throw new ArgumentException("Invalid Field"); }

            if (string.IsNullOrEmpty(item.Direction) || !_orders.Contains(item.Direction)) { throw new ArgumentException("Invalid Order Type"); }
        }

        if (sort.Any())
        {
            // "Alan1 asc, Alan2 desc" formatında sıralama ifadesi oluştur
            string ordering = string.Join(separator: ",", values: sort.Select(s => $"{s.Field} {s.Direction}"));
            return queryable.OrderBy(ordering);
        }

        return queryable;
    }

    /// <summary>
    /// Verilen filtre ve tüm iç içe alt filtrelerini düz bir liste olarak döndürür.
    /// </summary>
    /// <param name="filter">Kök filtre nesnesi.</param>
    /// <returns>Tüm filtreleri içeren düz liste.</returns>
    public static IList<Filter> GetAllFilters(Filter filter)
    {
        List<Filter> filters = new();
        GetFilters(filter, filters);
        return filters;
    }

    /// <summary>
    /// Filtreyi ve alt filtrelerini özyinelemeli olarak listeye ekler.
    /// </summary>
    private static void GetFilters(Filter filter, IList<Filter> filters)
    {
        filters.Add(filter);

        // Alt filtreler varsa özyinelemeli olarak işle
        if (filter.Filters is not null && filter.Filters.Any())
        {
            foreach (Filter item in filter.Filters)
            {
                GetFilters(item, filters);
            }
        }
    }

    /// <summary>
    /// Bir <see cref="Filter"/> nesnesini LINQ Dynamic Core'un anlayacağı where ifadesi string'ine dönüştürür.
    /// </summary>
    /// <param name="filter">Dönüştürülecek filtre.</param>
    /// <param name="filters">Tüm filtreleri içeren düz liste (parametre indeksi için kullanılır).</param>
    /// <returns>LINQ Dynamic Core formatında where ifadesi.</returns>
    public static string Transform(Filter filter, IList<Filter> filters)
    {
        if (string.IsNullOrEmpty(filter.Field)) { throw new ArgumentException("Invalid Field"); }

        if (string.IsNullOrEmpty(filter.Operator) || !_operators.ContainsKey(filter.Operator)) { throw new ArgumentException("Invalid Operator"); }

        // Filtrenin listedeki indeksi, parametre referansı (@0, @1, ...) olarak kullanılır
        int index = filters.IndexOf(filter);
        string comparison = _operators[filter.Operator];
        StringBuilder where = new StringBuilder();

        if (!string.IsNullOrEmpty(filter.Value))
        {
            // "doesnotcontain" için Contains metodunu olumsuz olarak uygula
            if (filter.Operator == "doesnotcontain") { where.Append($"(!np({filter.Field}).{comparison}(@{index.ToString()}))"); }
            // String metod operatörleri için metod çağrısı formatı kullan
            else if (comparison is "StartsWith" or "EndsWith" or "Contains") { where.Append($"(np({filter.Field}).{comparison}(@{index.ToString()}))"); }
            // Diğer operatörler için standart karşılaştırma formatı kullan
            else { where.Append($"np({filter.Field}) {comparison} @{index.ToString()}"); }
        }
        else if (filter.Operator is "isnull" or "isnotnull")
        {
            // Null kontrolü için değer parametresi gerekmez
            where.Append($"np({filter.Field}) {comparison}");
        }

        // Alt filtreler ve mantıksal bağlaç varsa, özyinelemeli olarak birleştir
        if (filter.Logic is not null && filter.Filters is not null && filter.Filters.Any())
        {
            if (!_logics.Contains(filter.Logic)) { throw new ArgumentException("Invalid Logic"); }
            return $"{where} {filter.Logic} ({string.Join(separator: $" {filter.Logic} ", value: filter.Filters.Select(f => Transform(f, filters)).ToArray())})";
        }

        return where.ToString();
    }

    #endregion
}
