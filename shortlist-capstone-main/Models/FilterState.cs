namespace Shortlist.Web.Models
{
    public class FilterState
    {
        public decimal? Budget { get; set; }
        public int? MaxDistanceKm { get; set; }

        // store selected priorities (max 3 enforced by UI/logic later)
        public List<string> Priorities { get; set; } = new();
    }
}
