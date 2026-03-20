namespace Shortlist.Web.Models
{
    public class RentalListingsPageViewModel
    {
        public RentalListingsSearchViewModel Search { get; set; } = new();
        public List<RentalListingCard> Listings { get; set; } = new();

        public bool HasResults => Listings.Count > 0;
        public string StatusMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}