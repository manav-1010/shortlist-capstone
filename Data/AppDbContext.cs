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

        // saved searches created by users.
        public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

        // per-user settings
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();

        // NEW: personal notes attached to results / compare places
        public DbSet<AreaNote> AreaNotes => Set<AreaNote>();

        // configures entity relationships and constraints.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ensuring each email address is unique.
            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // when user is deleted, cascade delete their saved searches.
            modelBuilder.Entity<SavedSearch>()
                .HasOne(s => s.User)
                .WithMany(u => u.SavedSearches)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // each user can only have ONE settings record.
            modelBuilder.Entity<UserSettings>()
                .HasIndex(x => x.UserId)
                .IsUnique();

            // One User = One settings record.
            modelBuilder.Entity<UserSettings>()
                .HasOne(x => x.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // NEW: one user can have many notes
            modelBuilder.Entity<AreaNote>()
                .HasOne(n => n.User)
                .WithMany(u => u.AreaNotes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // NEW: one user should only have one note per place
            modelBuilder.Entity<AreaNote>()
                .HasIndex(n => new { n.UserId, n.PlaceId })
                .IsUnique();

            modelBuilder.Entity<AreaNote>()
                .Property(n => n.PlaceId)
                .HasMaxLength(120);

            modelBuilder.Entity<AreaNote>()
                .Property(n => n.PlaceName)
                .HasMaxLength(160);

            modelBuilder.Entity<AreaNote>()
                .Property(n => n.Category)
                .HasMaxLength(80);

            modelBuilder.Entity<AreaNote>()
                .Property(n => n.NoteText)
                .HasMaxLength(2000);
        }

        // automatically populate timestamps before saving changes.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var savedSearchEntries = ChangeTracker
                .Entries<SavedSearch>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in savedSearchEntries)
            {
                if (entry.Entity.CreatedAtUtc == default)
                {
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                }
            }

            var noteEntries = ChangeTracker
                .Entries<AreaNote>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in noteEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.CreatedAtUtc == default)
                    {
                        entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    }
                }

                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}