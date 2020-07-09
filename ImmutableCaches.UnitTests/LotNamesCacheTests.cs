using System.Collections.Generic;
using System.Threading.Tasks;
using ImmutableCaches.Caches;
using ImmutableCaches.DbContext;
using ImmutableCaches.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ImmutableCaches.UnitTests
{
    [TestClass]
    public class LotNamesCacheTests
    {
        [TestMethod]
        public async Task ShouldSetGetCache()
        {
            var options = new DbContextOptionsBuilder<ImmutableCacheDbContext>()
                .UseInMemoryDatabase(databaseName: "LotNamesCacheTests")
                .Options;

            using (var dbContext = new ImmutableCacheDbContext(options))
            {
                var lot1 = Mock.Of<ParkingLot>(x => x.LotCode == "02" && x.LotName == "Lot 02");
                var lot2 = Mock.Of<ParkingLot>(x => x.LotCode == "12" && x.LotName == "Lot 12");
                var lot3 = Mock.Of<ParkingLot>(x => x.LotCode == "13" && x.LotName == "Lot 13");
                await dbContext.ParkingLots.AddRangeAsync(new List<ParkingLot> { lot1, lot2, lot3 });
                await dbContext.SaveChangesAsync();

                var cache = new MemoryCache(new MemoryCacheOptions());
                var service = new LotNamesCache(dbContext, cache);
                Assert.IsFalse(cache.TryGetValue(LotNamesCache.CacheKey, out _));   // no cache at the initial stage

                var cachedLots = await service.LotNamesDictionary();
                Assert.IsTrue(cache.TryGetValue(LotNamesCache.CacheKey, out _));    // stored cache in memory due to query
                Assert.AreEqual(3, cachedLots.Count);
                var result1 = await service.GetLotNameByLocationCode(lot1.LotCode);
                Assert.AreEqual(lot1.LotName, result1);

                cachedLots.Add("20", "Lot 20");
                Assert.AreEqual(3, cachedLots.Count);       // immutable dictionary will not change.

                service.RemoveCachedDictionary();
                Assert.IsFalse(cache.TryGetValue(LotNamesCache.CacheKey, out _));   // no cache in memory after removal
                var result2 = await service.GetLotNameByLocationCode(lot2.LotCode);
                Assert.AreEqual(lot2.LotName, result2);
            }
        }
    }
}
