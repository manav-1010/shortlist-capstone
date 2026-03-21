using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using Shortlist.Web.Services;

namespace Shortlist.Web.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IRentalListingsService _rentalListingsService;
        private readonly ILogger<ListingsController> _logger;

        public ListingsController(
            IRentalListingsService rentalListingsService,
            ILogger<ListingsController> logger)
        {
            _rentalListingsService = rentalListingsService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string? query = "", string? city = "Kitchener", string? province = "ON", int beds = 0, decimal? maxPrice = null)
        {
            ViewBag.Query = query ?? "";
            ViewBag.City = city ?? "Kitchener";
            ViewBag.Province = province ?? "ON";
            ViewBag.Beds = beds;
            ViewBag.MaxPrice = maxPrice?.ToString() ?? "";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? query = "", string? city = "Kitchener", string? province = "ON", int beds = 0, decimal? maxPrice = null)
        {
            try
            {
                var vm = new RentalListingsSearchViewModel
                {
                    Query = query ?? "",
                    City = city ?? "",
                    ProvinceOrState = province ?? "",
                    MinBeds = beds,
                    MaxPrice = maxPrice,
                    Page = 1,
                    Sort = "relevance"
                };

                var items = await _rentalListingsService.SearchRentalsAsync(vm);

                return Json(new
                {
                    ok = true,
                    count = items.Count,
                    items
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