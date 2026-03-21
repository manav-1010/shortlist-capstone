namespace Shortlist.Web.Models
{
    public class RentalListingsSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public string City { get; set; } = "Kitchener";
        public string ProvinceOrState { get; set; } = "ON";
        public int MinBeds { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public string Sort { get; set; } = "relevance";
    }
}