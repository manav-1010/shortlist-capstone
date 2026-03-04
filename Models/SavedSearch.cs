using System;
using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class SavedSearch
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public UserProfile? User { get; set; }

        [MaxLength(60)]
        public string Name { get; set; } = "Saved Search";

        [Required]
        public string FilterStateJson { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid ShareToken { get; set; }
    }
}