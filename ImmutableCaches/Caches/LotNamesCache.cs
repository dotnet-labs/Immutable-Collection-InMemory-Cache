using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImmutableCaches.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ImmutableCaches.Caches
{
    public interface ILotNamesCache
    {
        Task<ImmutableDictionary<string, string>> LotNamesDictionary();
        Task<string> GetLotNameByLocationCode(string locationCode);
        void RemoveCachedDictionary();
    }

    public class LotNamesCache : ILotNamesCache
    {
        public const string CacheKey = nameof(LotNamesCache);
        private readonly ImmutableCacheDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public LotNamesCache(ImmutableCacheDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<ImmutableDictionary<string, string>> LotNamesDictionary()
        {
            if (!_cache.TryGetValue(CacheKey, out ImmutableDictionary<string, string> result))
            {
                var lotNames = await _dbContext.ParkingLots.AsNoTracking().ToListAsync();
                result = lotNames.ToImmutableDictionary(x => x.LotCode, x => x.LotName);
                var options = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                _cache.Set(CacheKey, result, options);
            }

            return result;
        }

        public async Task<string> GetLotNameByLocationCode(string locationCode)
        {
            if (string.IsNullOrWhiteSpace(locationCode))
            {
                return null;
            }
            var dictionary = await LotNamesDictionary();
            return dictionary.GetValueOrDefault(locationCode);
        }

        public void RemoveCachedDictionary()
        {
            _cache.Remove(CacheKey);
        }
    }
}
