namespace Shortlist.Web.Models
{
    public class CompareItem
    {
        public string Id { get; set; } = "";          
        public string Name { get; set; } = "Unnamed";
        public string Category { get; set; } = "Other";

        public double DistKm { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}