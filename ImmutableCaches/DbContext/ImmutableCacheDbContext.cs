using ImmutableCaches.Models;
using Microsoft.EntityFrameworkCore;

namespace ImmutableCaches.DbContext
{
    public class ImmutableCacheDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public virtual DbSet<ParkingLot> ParkingLots { get; protected set; }

        public ImmutableCacheDbContext(DbContextOptions<ImmutableCacheDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkingLot>().HasKey(x => x.LotCode);
        }
    }
}
