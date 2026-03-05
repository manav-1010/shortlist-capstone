using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Models;

namespace Shortlist.Web.Data
{
    // SQLite is used as backing database for this project.
    public class AppDbContext : DbContext
    {
        // DbContext constructor used by ASP.NET dependency injection.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ---------- Database Tables ------------

        // user accounts for application.
        public DbSet<UserProfile> Users => Set<UserProfile>();

        // savd searches created by users.
        public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

        // per-user settings
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();

        // configures entity relationships and constraints.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ensuring each email address is unique.
            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // when user is deleted, cascade delete their saved searches and settings.
            modelBuilder.Entity<SavedSearch>()
                .HasOne(s => s.User)
                .WithMany(u => u.SavedSearches)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // each user can only have ONE settings record.
            modelBuilder.Entity<UserSettings>()
     .HasIndex(x => x.UserId)
     .IsUnique();

            // One USer = One settings record.
            modelBuilder.Entity<UserSettings>()
                .HasOne(x => x.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}