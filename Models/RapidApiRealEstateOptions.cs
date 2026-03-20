namespace Shortlist.Web.Models
{
    public class RapidApiRealEstateOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string SearchPath { get; set; } = string.Empty;
        public string DefaultCountry { get; set; } = "CA";
    }
}