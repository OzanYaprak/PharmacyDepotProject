using Application.Pipelines.Caching.Add;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Application.Pipelines.Caching.Remove;

public class CacheRemovingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>, ICacheRemoverRequest
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CacheRemovingBehavior<TRequest, TResponse>> _logger;

    public CacheRemovingBehavior(IDistributedCache distributedCache, ILogger<CacheRemovingBehavior<TRequest, TResponse>> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache)
            return await next();

        TResponse response = await next();

        // Tekil cache key silme
        if (request.CacheKey != null)
        {
            await _distributedCache.RemoveAsync(request.CacheKey, cancellationToken);
            _logger.LogInformation("Cache entry with key '{CacheKey}' has been removed.", request.CacheKey);
        }

        // Grup bazlı cache silme: gruptaki tüm key'leri sil
        if (request.CacheGroupKey != null)
        {
            await RemoveCacheGroupAsync(request.CacheGroupKey, cancellationToken);
        }

        return response;
    }

    private async Task RemoveCacheGroupAsync(string cacheGroupKey, CancellationToken cancellationToken)
    {
        byte[]? cachedGroupKeys = await _distributedCache.GetAsync(cacheGroupKey, cancellationToken);

        if (cachedGroupKeys == null)
        {
            _logger.LogInformation("Cache group '{CacheGroupKey}' not found, nothing to remove.", cacheGroupKey);
            return;
        }

        HashSet<string> groupKeys = JsonSerializer.Deserialize<HashSet<string>>(Encoding.UTF8.GetString(cachedGroupKeys)) ?? new HashSet<string>();

        foreach (string key in groupKeys)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            _logger.LogInformation("Cache entry '{CacheKey}' removed from group '{CacheGroupKey}'.", key, cacheGroupKey);
        }

        // Grup kaydını ve expiration metadata'sını temizle
        await _distributedCache.RemoveAsync(cacheGroupKey, cancellationToken);
        await _distributedCache.RemoveAsync($"{cacheGroupKey}_ExpirationTime", cancellationToken);

        _logger.LogInformation("Cache group '{CacheGroupKey}' has been fully cleared.", cacheGroupKey);
    }
}
