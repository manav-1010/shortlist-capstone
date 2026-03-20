using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public List<SavedSearch> SavedSearches { get; set; } = new();
        public UserSettings? Settings { get; set; }
        public List<AreaNote> AreaNotes { get; set; } = new();
    }
}