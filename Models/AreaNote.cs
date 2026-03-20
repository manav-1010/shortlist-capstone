using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortlist.Web.Models
{
    public class AreaNote
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(120)]
        public string PlaceId { get; set; } = string.Empty;

        [Required, MaxLength(160)]
        public string PlaceName { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Category { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }

        [MaxLength(2000)]
        public string NoteText { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserProfile? User { get; set; }
    }
}