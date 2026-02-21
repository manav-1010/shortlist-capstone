using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class SavedSearch
    {
        public int Id { get; set; }

        public int UserProfileId { get; set; }
        public UserProfile? UserProfile { get; set; }

        [MaxLength(60)]
        public string Name { get; set; } = "Saved Search";

        // Store the search filters as JSON (easy & flexible)
        [Required]
        public string FilterStateJson { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}