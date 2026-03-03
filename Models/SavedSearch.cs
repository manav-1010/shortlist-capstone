using System;
using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class SavedSearch
    {
        public int Id { get; set; }

        // Link to the logged-in User (the one you already authenticate)
        public int UserId { get; set; }
        public User? User { get; set; }

        [MaxLength(60)]
        public string Name { get; set; } = "Saved Search";

        // Store the search filters as JSON (easy & flexible)
        [Required]
        public string FilterStateJson { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}