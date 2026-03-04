using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class UserSettings
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public UserProfile? User { get; set; }

        [Range(1, 25)]
        public int DefaultRadiusKm { get; set; } = 3;

        // Store up to 3 priorities as CSV (simple + SQLite-friendly)
        [MaxLength(120)]
        public string DefaultPrioritiesCsv { get; set; } = "";

        [MaxLength(80)]
        public string? DefaultLocationLabel { get; set; }
    }
}