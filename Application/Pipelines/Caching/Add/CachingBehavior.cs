using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Application.Pipelines.Caching.Add;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>, ICacheableRequest
{
    private readonly CacheSettings _cacheSettings;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    public CachingBehavior(CacheSettings cacheSettings, IDistributedCache distributedCache, IConfiguration configuration, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() ?? cacheSettings;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling caching for request of type {RequestType} with cache key {CacheKey}", typeof(TRequest).Name, request.CacheKey);

        TResponse? response;
        // If the request has BypassCache set to true, skip caching and proceed to the next handler
        if (request.BypassCache.Equals(true))
        {
            return await next();
        }

        // Try to get the cached response
        byte[]? cachedResponse = await _distributedCache.GetAsync(request.CacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            // If a cached response exists, deserialize
            response = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedResponse))!;

            _logger.LogInformation($"Fetched from cache -> {request.CacheKey}");
            _logger.LogInformation("Cache expiration time: {CacheExpirationTime}", request.CacheExpiration ?? TimeSpan.FromDays(_cacheSettings.ExpirationTime));
        }
        else // If no cached response exists, proceed to the next handler and cache the response
        {
            response = await GetResponseAndAddToCache(request, next, cancellationToken);

            _logger.LogInformation($"Added to cache -> {request.CacheKey}");
            _logger.LogInformation("Cache expiration time: {CacheExpirationTime}", request.CacheExpiration ?? TimeSpan.FromDays(_cacheSettings.ExpirationTime));
        }


        return response!;
    }

    private async Task<TResponse?> GetResponseAndAddToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next(); // Get the response from the next handler
        TimeSpan cacheExpirationTime = request.CacheExpiration ?? TimeSpan.FromDays(_cacheSettings.ExpirationTime); // Use the request's CacheExpiration if provided, otherwise use the default from appsettings

        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = cacheExpirationTime, // Set the sliding expiration time for the cache entry
        };

        byte[] serializedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)); // Serialize the response to a byte array
        await _distributedCache.SetAsync(request.CacheKey, serializedData, cacheEntryOptions, cancellationToken); // Add the serialized response to the cache

        if (request.CacheGroupKey != null)
        {
            await AddCacheKeyToGroup(request, cacheExpirationTime, cancellationToken);
        }

        return response;
    }

    private async Task AddCacheKeyToGroup(TRequest request, TimeSpan cacheExpirationTime, CancellationToken cancellationToken)
    {
        byte[]? cachedGroupKeys = await _distributedCache.GetAsync(request.CacheGroupKey!, cancellationToken);

        HashSet<string> groupKeys;

        if (cachedGroupKeys != null)
        {
            groupKeys = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cachedGroupKeys)) ?? new HashSet<string>();

            if (!groupKeys.Contains(request.CacheKey))
            {
                groupKeys.Add(request.CacheKey);

                byte[] updatedGroupKeys = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(groupKeys));

                byte[] cachedGroupCacheExpirationTime = await _distributedCache.GetAsync($"{request.CacheGroupKey}_ExpirationTime", cancellationToken) ?? Array.Empty<byte>();
                int cachedGroupCacheExpirationTimeValue = cachedGroupCacheExpirationTime.Length > 0
                    ? Convert.ToInt32(Encoding.UTF8.GetString(cachedGroupCacheExpirationTime))
                    : Convert.ToInt32(cacheExpirationTime.TotalSeconds);

                DistributedCacheEntryOptions updateOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(cachedGroupCacheExpirationTimeValue),
                };

                await _distributedCache.SetAsync(request.CacheGroupKey!, updatedGroupKeys, updateOptions, cancellationToken);
                _logger.LogInformation($"Updated cache group {request.CacheGroupKey} with new key {request.CacheKey}");
            }
        }
        else
        {
            groupKeys = new HashSet<string>(new[] { request.CacheKey });
            byte[] newCachedGroupKeys = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(groupKeys));

            byte[] cachedGroupCacheExpirationTime = await _distributedCache.GetAsync($"{request.CacheGroupKey}_ExpirationTime", cancellationToken) ?? Array.Empty<byte>();

            int cachedGroupCacheExpirationTimeValue = 0;
            if (cachedGroupCacheExpirationTime.Length > 0) { cachedGroupCacheExpirationTimeValue = Convert.ToInt32(Encoding.UTF8.GetString(cachedGroupCacheExpirationTime)); }
            if (cachedGroupCacheExpirationTimeValue == 0 || cachedGroupCacheExpirationTimeValue < cacheExpirationTime.TotalSeconds) { cachedGroupCacheExpirationTimeValue = Convert.ToInt32(cacheExpirationTime.TotalSeconds); }

            byte[] serializeCachedGroupCacheExpirationTime = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedGroupCacheExpirationTimeValue));

            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(Convert.ToDouble(cachedGroupCacheExpirationTimeValue)),
            };

            await _distributedCache.SetAsync(request.CacheGroupKey!, newCachedGroupKeys, cacheEntryOptions, cancellationToken);
            _logger.LogInformation($"Added cache key {request.CacheKey} to cache group {request.CacheGroupKey}");

            await _distributedCache.SetAsync($"{request.CacheGroupKey}_ExpirationTime", serializeCachedGroupCacheExpirationTime, cacheEntryOptions, cancellationToken);
            _logger.LogInformation($"Set cache group {request.CacheGroupKey} expiration time to {cacheEntryOptions.SlidingExpiration}");
        }
    }
}
