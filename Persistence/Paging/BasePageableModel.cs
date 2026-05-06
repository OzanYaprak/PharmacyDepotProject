namespace Persistence.Paging;

public abstract class BasePageableModel
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int DataCount { get; set; }
    public int DeletedDataCount { get; set; }
    public int TotalPages { get; set; }
    public IList<object>? DataList { get; set; } 
    public bool HasPreviousPage { get; set; }
    public bool HasContinuousPage { get; set; }
}
