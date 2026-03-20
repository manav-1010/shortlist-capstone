namespace Shortlist.Web.Models
{
    public class RentalListingCard
    {
        public string ListingId { get; set; } = string.Empty;
        public string Title { get; set; } = "Listing";
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ProvinceOrState { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public decimal? Price { get; set; }
        public string Currency { get; set; } = "CAD";

        public int? Bedrooms { get; set; }
        public decimal? Bathrooms { get; set; }
        public int? LivingArea { get; set; }
        public string LivingAreaUnit { get; set; } = "sqft";

        public string PropertyType { get; set; } = string.Empty;
        public string Status { get; set; } = "For Rent";
        public string DetailUrl { get; set; } = string.Empty;

        public string PrimaryImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string SourceName { get; set; } = "RapidAPI";
    }
}