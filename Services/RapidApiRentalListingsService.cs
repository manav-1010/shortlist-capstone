using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Shortlist.Web.Models;

namespace Shortlist.Web.Services
{
    public class RapidApiRentalListingsService : IRentalListingsService
    {
        private readonly HttpClient _httpClient;
        private readonly RapidApiRealEstateOptions _options;
        private readonly ILogger<RapidApiRentalListingsService> _logger;

        public RapidApiRentalListingsService(
            HttpClient httpClient,
            IOptions<RapidApiRealEstateOptions> options,
            ILogger<RapidApiRentalListingsService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<List<RentalListingCard>> SearchRentalsAsync(
            RentalListingsSearchViewModel search,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey) ||
                _options.ApiKey.Contains("PASTE_YOUR_RAPIDAPI_KEY_HERE", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("RapidAPI configuration is missing. Check appsettings.Development.json.");
            }

            if (string.IsNullOrWhiteSpace(_options.BaseUrl) || string.IsNullOrWhiteSpace(_options.SearchPath))
            {
                throw new InvalidOperationException("RapidAPI real-estate configuration is incomplete.");
            }

            var location = BuildLocation(search);
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new InvalidOperationException("Please provide a search query or city.");
            }

            // START WITH MINIMAL PARAMS ONLY
            var query = new Dictionary<string, string?>
            {
                ["location"] = location,
                ["page"] = search.Page <= 0 ? "1" : search.Page.ToString(CultureInfo.InvariantCulture)
            };

            var url = QueryHelpers.AddQueryString(
                $"{_options.BaseUrl.TrimEnd('/')}{_options.SearchPath}",
                query!);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("X-RapidAPI-Key", _options.ApiKey);
            request.Headers.TryAddWithoutValidation("X-RapidAPI-Host", _options.Host);

            _logger.LogInformation("RapidAPI request URL: {Url}", url);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogInformation("RapidAPI response status: {StatusCode}", (int)response.StatusCode);
            _logger.LogInformation("RapidAPI response body: {Body}", json);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"RapidAPI request failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {json}");
            }

            using var doc = JsonDocument.Parse(json);
            return ParseListings(doc.RootElement);
        }

        private string BuildLocation(RentalListingsSearchViewModel search)
        {
            if (!string.IsNullOrWhiteSpace(search.Query))
            {
                return search.Query.Trim();
            }

            var city = search.City?.Trim();
            var province = search.ProvinceOrState?.Trim();
            var country = _options.DefaultCountry?.Trim();

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(city)) parts.Add(city);
            if (!string.IsNullOrWhiteSpace(province)) parts.Add(province);
            if (!string.IsNullOrWhiteSpace(country)) parts.Add(country);

            return string.Join(", ", parts);
        }

        private List<RentalListingCard> ParseListings(JsonElement root)
        {
            var results = new List<RentalListingCard>();

            JsonElement listingsArray = default;
            var found =
                TryGetArray(root, "properties", out listingsArray) ||
                TryGetArray(root, "results", out listingsArray) ||
                TryGetNestedArray(root, "data", "properties", out listingsArray) ||
                TryGetNestedArray(root, "data", "results", out listingsArray) ||
                TryGetNestedArray(root, "data", "home_search", "results", out listingsArray);

            if (!found || listingsArray.ValueKind != JsonValueKind.Array)
            {
                return results;
            }

            foreach (var item in listingsArray.EnumerateArray())
            {
                var card = new RentalListingCard
                {
                    ListingId = GetString(item, "property_id")
                                ?? GetString(item, "listing_id")
                                ?? GetString(item, "zpid")
                                ?? Guid.NewGuid().ToString("N"),

                    Title = BuildTitle(item),
                    AddressLine = GetNestedString(item, "address", "streetAddress")
                                  ?? GetString(item, "address")
                                  ?? GetString(item, "streetAddress")
                                  ?? string.Empty,

                    City = GetNestedString(item, "address", "city")
                           ?? GetString(item, "city")
                           ?? string.Empty,

                    ProvinceOrState = GetNestedString(item, "address", "state")
                                      ?? GetString(item, "state")
                                      ?? string.Empty,

                    PostalCode = GetNestedString(item, "address", "zipcode")
                                 ?? GetString(item, "postal_code")
                                 ?? GetString(item, "zipcode")
                                 ?? string.Empty,

                    Price = GetDecimal(item, "price")
                            ?? GetDecimal(item, "list_price")
                            ?? GetDecimal(item, "rent"),

                    Currency = GetString(item, "currency") ?? "CAD",

                    Bedrooms = GetInt(item, "bedrooms")
                               ?? GetInt(item, "beds"),

                    Bathrooms = GetDecimal(item, "bathrooms")
                                ?? GetDecimal(item, "baths"),

                    LivingArea = GetInt(item, "livingArea")
                                 ?? GetInt(item, "living_area")
                                 ?? GetInt(item, "sqft"),

                    PropertyType = GetString(item, "propertyType")
                                   ?? GetString(item, "property_type")
                                   ?? GetString(item, "homeType")
                                   ?? "Rental",

                    Status = GetString(item, "homeStatus")
                             ?? GetString(item, "status")
                             ?? "For Rent",

                    DetailUrl = BuildDetailUrl(item),

                    Latitude = GetDouble(item, "latitude")
                               ?? GetDouble(item, "lat"),

                    Longitude = GetDouble(item, "longitude")
                                ?? GetDouble(item, "lon")
                                ?? GetDouble(item, "lng"),

                    SourceName = "RapidAPI"
                };

                card.ImageUrls = ExtractImages(item);
                card.PrimaryImageUrl = card.ImageUrls.FirstOrDefault() ?? "";

                results.Add(card);
            }

            return results;
        }

        private static string BuildTitle(JsonElement item)
        {
            var explicitTitle = GetString(item, "title") ?? GetString(item, "listingTitle");
            if (!string.IsNullOrWhiteSpace(explicitTitle))
                return explicitTitle;

            var beds = GetInt(item, "bedrooms") ?? GetInt(item, "beds");
            var baths = GetDecimal(item, "bathrooms") ?? GetDecimal(item, "baths");
            var type = GetString(item, "propertyType") ?? GetString(item, "property_type") ?? "Rental";

            var pieces = new List<string>();
            if (beds.HasValue) pieces.Add($"{beds.Value} bd");
            if (baths.HasValue) pieces.Add($"{baths.Value:0.#} ba");
            pieces.Add(type);

            return string.Join(" • ", pieces);
        }

        private static string BuildDetailUrl(JsonElement item)
        {
            var url = GetString(item, "detailUrl")
                      ?? GetString(item, "property_url")
                      ?? GetString(item, "url")
                      ?? GetString(item, "hdpUrl");

            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return url;

            return $"https://www.zillow.com{url}";
        }

        private static List<string> ExtractImages(JsonElement item)
        {
            var images = new List<string>();

            if (item.TryGetProperty("primaryPhoto", out var primaryPhoto))
            {
                var url = GetString(primaryPhoto, "url");
                if (!string.IsNullOrWhiteSpace(url))
                    images.Add(url);
            }

            if (item.TryGetProperty("responsivePhotos", out var responsivePhotos)
                && responsivePhotos.ValueKind == JsonValueKind.Array)
            {
                foreach (var photo in responsivePhotos.EnumerateArray())
                {
                    var url = GetString(photo, "url");
                    if (!string.IsNullOrWhiteSpace(url))
                        images.Add(url);
                }
            }

            if (item.TryGetProperty("photos", out var photos)
                && photos.ValueKind == JsonValueKind.Array)
            {
                foreach (var photo in photos.EnumerateArray())
                {
                    if (photo.ValueKind == JsonValueKind.String)
                    {
                        var url = photo.GetString();
                        if (!string.IsNullOrWhiteSpace(url))
                            images.Add(url);
                    }
                    else
                    {
                        var url = GetString(photo, "url") ?? GetString(photo, "href");
                        if (!string.IsNullOrWhiteSpace(url))
                            images.Add(url);
                    }
                }
            }

            return images.Distinct().Take(8).ToList();
        }

        private static bool TryGetArray(JsonElement root, string propertyName, out JsonElement array)
        {
            array = default;
            if (root.TryGetProperty(propertyName, out var p) && p.ValueKind == JsonValueKind.Array)
            {
                array = p;
                return true;
            }

            return false;
        }

        private static bool TryGetNestedArray(JsonElement root, string parent, string child, out JsonElement array)
        {
            array = default;
            if (root.TryGetProperty(parent, out var p)
                && p.ValueKind == JsonValueKind.Object
                && p.TryGetProperty(child, out var c)
                && c.ValueKind == JsonValueKind.Array)
            {
                array = c;
                return true;
            }

            return false;
        }

        private static bool TryGetNestedArray(JsonElement root, string parent, string child1, string child2, out JsonElement array)
        {
            array = default;
            if (root.TryGetProperty(parent, out var p)
                && p.ValueKind == JsonValueKind.Object
                && p.TryGetProperty(child1, out var c1)
                && c1.ValueKind == JsonValueKind.Object
                && c1.TryGetProperty(child2, out var c2)
                && c2.ValueKind == JsonValueKind.Array)
            {
                array = c2;
                return true;
            }

            return false;
        }

        private static string? GetNestedString(JsonElement root, string parent, string child)
        {
            if (root.TryGetProperty(parent, out var p)
                && p.ValueKind == JsonValueKind.Object
                && p.TryGetProperty(child, out var c))
            {
                return c.ValueKind == JsonValueKind.String ? c.GetString() : c.ToString();
            }

            return null;
        }

        private static string? GetString(JsonElement root, string name)
        {
            if (!root.TryGetProperty(name, out var p))
                return null;

            return p.ValueKind switch
            {
                JsonValueKind.String => p.GetString(),
                JsonValueKind.Number => p.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null
            };
        }

        private static decimal? GetDecimal(JsonElement root, string name)
        {
            if (!root.TryGetProperty(name, out var p))
                return n

            if (p.ValueKind == JsonValueKind.Number && p.TryGetDecimal(out var d))
                return d;

            if (p.ValueKind == JsonValueKind.String && decimal.TryParse(p.GetString(), out var s))
                return s;

            return null;
        }

        private static int? GetInt(JsonElement root, string name)
        {
            if (!root.TryGetProperty(name, out var p))
                return null;

            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var i))
                return i;

            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var s))
                return s;

            return null;
        }

        private static double? GetDouble(JsonElement root, string name)
        {
            if (!root.TryGetProperty(name, out var p))
                return null;

            if (p.ValueKind == JsonValueKind.Number && p.TryGetDouble(out var d))
                return d;

            if (p.ValueKind == JsonValueKind.String && double.TryParse(p.GetString(), out var s))
                return s;

            return null;
        }
    }
}