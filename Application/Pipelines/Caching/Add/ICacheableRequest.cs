namespace Application.Pipelines.Caching.Add;

public interface ICacheableRequest
{
    string CacheKey { get; }
    bool BypassCache { get; }
    string? CacheGroupKey { get; }
    TimeSpan? CacheExpiration { get; }
}
