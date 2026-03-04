using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Models;

namespace Shortlist.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserProfile> Users => Set<UserProfile>();
        public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<SavedSearch>()
                .HasOne(s => s.User)
                .WithMany(u => u.SavedSearches)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}