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
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("RapidAPI key is missing.");

            if (string.IsNullOrWhiteSpace(_options.Host))
                throw new InvalidOperationException("RapidAPI host is missing.");

            if (string.IsNullOrWhiteSpace(_options.BaseUrl))
                throw new InvalidOperationException("RapidAPI base URL is missing.");

            if (string.IsNullOrWhiteSpace(_options.SearchPath))
                throw new InvalidOperationException("RapidAPI search path is missing.");

            var attempts = BuildAttempts(search);

            foreach (var attempt in attempts)
            {
                var items = await ExecuteAttemptAsync(attempt, cancellationToken);
                if (items.Count > 0)
                    return items;
            }

            return new List<RentalListingCard>();
        }

        private List<Dictionary<string, string?>> BuildAttempts(RentalListingsSearchViewModel search)
        {
            var attempts = new List<Dictionary<string, string?>>();

            var query = (search.Query ?? string.Empty).Trim();
            var city = (search.City ?? string.Empty).Trim();
            var province = (search.ProvinceOrState ?? string.Empty).Trim();

            var locationFromFields = string.Join(", ", new[] { city, province }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

            if (!string.IsNullOrWhiteSpace(query))
            {
                attempts.Add(new Dictionary<string, string?>
                {
                    ["location"] = query,
                    ["page"] = "1"
                });
            }

            if (!string.IsNullOrWhiteSpace(locationFromFields))
            {
                attempts.Add(new Dictionary<string, string?>
                {
                    ["location"] = locationFromFields,
                    ["page"] = "1"
                });
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                attempts.Add(new Dictionary<string, string?>
                {
                    ["query"] = query,
                    ["page"] = "1"
                });
            }

            if (!string.IsNullOrWhiteSpace(locationFromFields))
            {
                attempts.Add(new Dictionary<string, string?>
                {
                    ["query"] = locationFromFields,
                    ["page"] = "1"
                });
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                attempts.Add(new Dictionary<string, string?>
                {
                    ["location"] = query,
                    ["status"] = "for_rent",
                    ["page"] = "1"
                });
            }

            if (!string.IsNullOrWhiteSpace(locationFromFields))
            {
                attempts.Add(new Dictionary<string, string?>
                {
                    ["location"] = locationFromFields,
                    ["status"] = "for_rent",
                    ["page"] = "1"
                });
            }

            return attempts;
        }

        private async Task<List<RentalListingCard>> ExecuteAttemptAsync(
            Dictionary<string, string?> queryParams,
            CancellationToken cancellationToken)
        {
            var url = QueryHelpers.AddQueryString(
                $"{_options.BaseUrl.TrimEnd('/')}{_options.SearchPath}",
                queryParams!);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("X-RapidAPI-Key", _options.ApiKey);
            request.Headers.TryAddWithoutValidation("X-RapidAPI-Host", _options.Host);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "RapidAPI request failed. Status {StatusCode}. Url: {Url}. Body: {Body}",
                    (int)response.StatusCode, url, json);

                return new List<RentalListingCard>();
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                var items = ParseListings(doc.RootElement);

                _logger.LogInformation("RapidAPI returned {Count} items for url {Url}", items.Count, url);
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse RapidAPI response for url {Url}", url);
                return new List<RentalListingCard>();
            }
        }

        private List<RentalListingCard> ParseListings(JsonElement root)
        {
            var results = new List<RentalListingCard>();

            JsonElement listingsArray = default;
            var found =
                TryGetArray(root, "results", out listingsArray) ||
                TryGetArray(root, "properties", out listingsArray) ||
                TryGetArray(root, "data", out listingsArray) ||
                TryGetNestedArray(root, "data", "results", out listingsArray) ||
                TryGetNestedArray(root, "data", "properties", out listingsArray) ||
                TryGetDoubleNestedArray(root, "data", "home_search", "results", out listingsArray) ||
                TryGetDoubleNestedArray(root, "data", "listings", "results", out listingsArray);

            if (!found || listingsArray.ValueKind != JsonValueKind.Array)
                return results;

            foreach (var item in listingsArray.EnumerateArray())
            {
                var card = new RentalListingCard
                {
                    ListingId =
                        GetString(item, "listing_id") ??
                        GetString(item, "property_id") ??
                        GetString(item, "id") ??
                        Guid.NewGuid().ToString("N"),

                    Title =
                        GetString(item, "title") ??
                        GetString(item, "listing_title") ??
                        BuildTitle(item),

                    AddressLine =
                        GetString(item, "address") ??
                        GetNestedString(item, "location", "address") ??
                        GetNestedString(item, "address", "line") ??
                        GetNestedString(item, "address", "streetAddress") ??
                        string.Empty,

                    City =
                        GetString(item, "city") ??
                        GetNestedString(item, "location", "city") ??
                        GetNestedString(item, "address", "city") ??
                        string.Empty,

                    ProvinceOrState =
                        GetString(item, "state") ??
                        GetString(item, "province") ??
                        GetNestedString(item, "location", "state") ??
                        GetNestedString(item, "address", "state") ??
                        string.Empty,

                    PostalCode =
                        GetString(item, "postal_code") ??
                        GetString(item, "zipcode") ??
                        GetNestedString(item, "address", "zipcode") ??
                        string.Empty,

                    Price =
                        GetDecimal(item, "price") ??
                        GetDecimal(item, "list_price") ??
                        GetDecimal(item, "price_per_month"),

                    Currency = GetString(item, "currency") ?? "CAD",

                    Bedrooms =
                        GetInt(item, "beds") ??
                        GetInt(item, "bedrooms"),

                    Bathrooms =
                        GetDecimal(item, "baths") ??
                        GetDecimal(item, "bathrooms"),

                    LivingArea =
                        GetInt(item, "sqft") ??
                        GetInt(item, "living_area"),

                    PropertyType =
                        GetString(item, "property_type") ??
                        GetString(item, "home_type") ??
                        GetString(item, "type") ??
                        "Rental",

                    Status =
                        GetString(item, "status") ??
                        GetString(item, "home_status") ??
                        "Available",

                    DetailUrl =
                        GetString(item, "detail_url") ??
                        GetString(item, "url") ??
                        GetString(item, "listing_url") ??
                        GetString(item, "href") ??
                        string.Empty,

                    Latitude =
                        GetDouble(item, "latitude") ??
                        GetDouble(item, "lat"),

                    Longitude =
                        GetDouble(item, "longitude") ??
                        GetDouble(item, "lng"),

                    SourceName = "RapidAPI"
                };

                card.ImageUrls = ExtractImages(item);
                card.PrimaryImageUrl = card.ImageUrls.FirstOrDefault() ?? string.Empty;

                results.Add(card);
            }

            return results;
        }

        private static string BuildTitle(JsonElement item)
        {
            var beds = GetInt(item, "beds") ?? GetInt(item, "bedrooms");
            var baths = GetDecimal(item, "baths") ?? GetDecimal(item, "bathrooms");
            var type = GetString(item, "property_type") ?? GetString(item, "type") ?? "Rental";

            var parts = new List<string>();
            if (beds.HasValue) parts.Add($"{beds.Value} bd");
            if (baths.HasValue) parts.Add($"{baths.Value:0.#} ba");
            parts.Add(type);

            return string.Join(" • ", parts);
        }

        private static List<string> ExtractImages(JsonElement item)
        {
            var images = new List<string>();

            if (item.TryGetProperty("photos", out var photos) && photos.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in photos.EnumerateArray())
                {
                    if (p.ValueKind == JsonValueKind.String)
                    {
                        var s = p.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            images.Add(s);
                    }
                    else
                    {
                        var url = GetString(p, "url") ?? GetString(p, "href");
                        if (!string.IsNullOrWhiteSpace(url))
                            images.Add(url);
                    }
                }
            }

            if (item.TryGetProperty("responsivePhotos", out var responsivePhotos) &&
                responsivePhotos.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in responsivePhotos.EnumerateArray())
                {
                    var url = GetString(p, "url") ?? GetString(p, "href");
                    if (!string.IsNullOrWhiteSpace(url))
                        images.Add(url);
                }
            }

            if (item.TryGetProperty("listing_photos", out var listingPhotos) &&
                listingPhotos.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in listingPhotos.EnumerateArray())
                {
                    var url = GetString(p, "url") ?? GetString(p, "href");
                    if (!string.IsNullOrWhiteSpace(url))
                        images.Add(url);
                }
            }

            var primary =
                GetString(item, "primary_photo") ??
                GetString(item, "thumbnail") ??
                GetString(item, "photo") ??
                GetString(item, "image_url") ??
                GetString(item, "img_url") ??
                GetNestedString(item, "primaryPhoto", "url") ??
                GetNestedString(item, "primaryPhoto", "href");

            if (!string.IsNullOrWhiteSpace(primary))
                images.Insert(0, primary);

            return images
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .Take(8)
                .ToList();
        }

        private static bool TryGetArray(JsonElement root, string propertyName, out JsonElement array)
        {
            array = default;
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty(propertyName, out var p) &&
                p.ValueKind == JsonValueKind.Array)
            {
                array = p;
                return true;
            }

            return false;
        }

        private static bool TryGetNestedArray(JsonElement root, string parent, string child, out JsonElement array)
        {
            array = default;
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty(parent, out var p) &&
                p.ValueKind == JsonValueKind.Object &&
                p.TryGetProperty(child, out var c) &&
                c.ValueKind == JsonValueKind.Array)
            {
                array = c;
                return true;
            }

            return false;
        }

        private static bool TryGetDoubleNestedArray(JsonElement root, string a, string b, string c, out JsonElement array)
        {
            array = default;
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty(a, out var p1) &&
                p1.ValueKind == JsonValueKind.Object &&
                p1.TryGetProperty(b, out var p2) &&
                p2.ValueKind == JsonValueKind.Object &&
                p2.TryGetProperty(c, out var p3) &&
                p3.ValueKind == JsonValueKind.Array)
            {
                array = p3;
                return true;
            }

            return false;
        }

        private static string? GetNestedString(JsonElement root, string parent, string child)
        {
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty(parent, out var p) &&
                p.ValueKind == JsonValueKind.Object &&
                p.TryGetProperty(child, out var c))
            {
                return c.ValueKind == JsonValueKind.String ? c.GetString() : c.ToString();
            }

            return null;
        }

        private static string? GetString(JsonElement root, string name)
        {
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(name, out var p))
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
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(name, out var p))
                return null;

            if (p.ValueKind == JsonValueKind.Number && p.TryGetDecimal(out var d))
                return d;

            if (p.ValueKind == JsonValueKind.String && decimal.TryParse(p.GetString(), out var s))
                return s;

            return null;
        }

        private static int? GetInt(JsonElement root, string name)
        {
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(name, out var p))
                return null;

            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var i))
                return i;

            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var s))
                return s;

            return null;
        }

        private static double? GetDouble(JsonElement root, string name)
        {
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(name, out var p))
                return null;

            if (p.ValueKind == JsonValueKind.Number && p.TryGetDouble(out var d))
                return d;

            if (p.ValueKind == JsonValueKind.String && double.TryParse(p.GetString(), out var s))
                return s;

            return null;
        }
    }
}