using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shortlist.Web.Models;
using Shortlist.Web.Services;

namespace Shortlist.Web.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IRentalListingsService _rentalListingsService;
        private readonly RapidApiRealEstateOptions _options;
        private readonly ILogger<ListingsController> _logger;

        public ListingsController(
            IRentalListingsService rentalListingsService,
            IOptions<RapidApiRealEstateOptions> options,
            ILogger<ListingsController> logger)
        {
            _rentalListingsService = rentalListingsService;
            _options = options.Value;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string? query = "", string? city = "Kitchener", string? province = "ON", int beds = 0, decimal? maxPrice = null)
        {
            ViewBag.Query = query ?? "";
            ViewBag.City = city ?? "Kitchener";
            ViewBag.Province = province ?? "ON";
            ViewBag.Beds = beds;
            ViewBag.MaxPrice = maxPrice;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? query = "", string? city = "Kitchener", string? province = "ON", int beds = 0, decimal? maxPrice = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_options.ApiKey) ||
                    _options.ApiKey.Contains("PASTE_YOUR", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        ok = false,
                        message = "RapidAPI configuration is missing or still using the placeholder key."
                    });
                }

                var vm = new RentalListingsSearchViewModel
                {
                    Query = query ?? "",
                    City = city ?? "Kitchener",
                    ProvinceOrState = province ?? "ON",
                    MinBeds = beds,
                    MaxPrice = maxPrice,
                    Page = 1,
                    Sort = "relevance"
                };

                var results = await _rentalListingsService.SearchRentalsAsync(vm);

                return Json(new
                {
                    ok = true,
                    count = results.Count,
                    items = results.Select(x => new
                    {
                        id = x.ListingId,
                        title = x.Title,
                        address = x.AddressLine,
                        city = x.City,
                        province = x.ProvinceOrState,
                        price = x.Price.HasValue ? $"${x.Price.Value:N0}" : "Price not listed",
                        beds = x.Bedrooms,
                        baths = x.Bathrooms,
                        image = x.PrimaryImageUrl,
                        detailUrl = x.DetailUrl,
                        propertyType = x.PropertyType
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Listings search failed.");

                return Json(new
                {
                    ok = false,
                    message = ex.Message
                });
            }
        }
    }
}