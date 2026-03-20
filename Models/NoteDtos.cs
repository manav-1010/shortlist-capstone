using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class SaveNoteRequest
    {
        [Required]
        [MaxLength(40)]
        public string TargetType { get; set; } = "Place";

        [Required]
        [MaxLength(120)]
        public string TargetId { get; set; } = string.Empty;

        [MaxLength(160)]
        public string? Title { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Maybe";

        [MaxLength(2000)]
        public string? NoteText { get; set; }
    }

    public class NoteCardDto
    {
        public string TargetType { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Status { get; set; } = "Maybe";
        public string? NoteText { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}