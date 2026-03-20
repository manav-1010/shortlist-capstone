namespace Shortlist.Web.Models
{
    public class NoteUpsertViewModel
    {
        public string PlaceId { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string? NoteText { get; set; }
    }
}