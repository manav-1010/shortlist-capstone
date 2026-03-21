namespace Shortlist.Web.Models
{
    public class RentalListingCard
    {
        public string ListingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ProvinceOrState { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public decimal? Price { get; set; }
        public string Currency { get; set; } = "CAD";

        public int? Bedrooms { get; set; }
        public decimal? Bathrooms { get; set; }
        public int? LivingArea { get; set; }

        public string PropertyType { get; set; } = "Rental";
        public string Status { get; set; } = "For Rent";

        public string DetailUrl { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<string> ImageUrls { get; set; } = new();
        public string PrimaryImageUrl { get; set; } = string.Empty;

        public string SourceName { get; set; } = "RapidAPI";
    }
}