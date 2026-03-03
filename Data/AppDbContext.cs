using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Models;

namespace Shortlist.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();
    }
}