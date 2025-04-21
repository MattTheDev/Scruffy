using Microsoft.EntityFrameworkCore;
using Scruffy.Data.Entities;

namespace Scruffy.Data
{
    public class ScruffyDbContext : DbContext
    {
        public DbSet<Channel> Channels => Set<Channel>();
protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var databasePath = Path.Combine(AppContext.BaseDirectory, "scruffy.db");
            options.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Channel>().Property(p => p.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Channel>().Property(p => p.ModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
