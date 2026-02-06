using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Shortlist.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // sample user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "test@shortlist.com",
                    Password = "password123",
                    Name = "Test User"
                }
            );
        }
    }
}