using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortlist.Web.Models
{
    public class UserNote
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(40)]
        public string TargetType { get; set; } = "Place"; // Place / Provider / Search

        [Required]
        [MaxLength(120)]
        public string TargetId { get; set; } = string.Empty; // e.g. node:12345

        [MaxLength(160)]
        public string? Title { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Maybe"; // Interested / Maybe / Not for me

        [MaxLength(2000)]
        public string? NoteText { get; set; }

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public UserProfile? User { get; set; }
    }
}