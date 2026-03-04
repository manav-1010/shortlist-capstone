namespace Shortlist.Web.Models
{
    public class CompareItem
    {
        public string Id { get; set; } = "";          // stable client id like "node:123"
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";    // PARKING / TRANSIT / etc
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double DistKm { get; set; }            // distance from selected center
    }
}