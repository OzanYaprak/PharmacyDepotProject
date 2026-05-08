namespace Application.Common.Requests;

/// <summary>
/// Sayfalama isteği için kullanılan ortak model.
/// </summary>
public class PageRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
