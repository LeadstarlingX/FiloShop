using Microsoft.Extensions.Caching.Distributed;

namespace FiloShop.Infrastructure.Services.Providers.Caching;

public static class CacheOptions
{
    private static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration)
    {
        return expiration is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : DefaultExpiration;
    }
}