using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = "";

        [Required, MaxLength(120)]
        public string Email { get; set; } = "";

        [Required, MaxLength(120)]
        public string Password { get; set; } = "";

        public List<SavedSearch> SavedSearches { get; set; } = new();
        public UserSettings? Settings { get; set; }
    }
}