namespace Shortlist.Web.Models
{
    public class RentalListingsSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public string City { get; set; } = "Kitchener";
        public string ProvinceOrState { get; set; } = "ON";
        public int Page { get; set; } = 1;
        public int MaxPrice { get; set; } = 0;
        public int MinBeds { get; set; } = 0;
        public string Sort { get; set; } = "relevance";
    }
}