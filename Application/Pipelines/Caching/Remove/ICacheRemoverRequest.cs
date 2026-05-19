namespace Application.Pipelines.Caching.Remove;

public interface ICacheRemoverRequest
{
    string? CacheKey { get; }
    string? CacheGroupKey { get; }
    bool BypassCache { get; }
}
