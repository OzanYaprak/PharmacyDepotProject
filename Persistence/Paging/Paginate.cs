namespace Persistence.Paging;

public class Paginate<T>
{
    public Paginate()
    {
        DataList = Array.Empty<T>();
    }


    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int DataCount { get; set; }
    public int TotalPages { get; set; }
    public IList<T>? DataList { get; set; }

    public bool HasPreviousPage => PageNumber > 0;
    public bool HasContinuousPage => PageNumber + 1 < TotalPages;
}
