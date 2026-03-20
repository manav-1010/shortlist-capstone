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
        public async Task<IActionResult> Index(
            string? query,
            string? city = "Kitchener",
            string? provinceOrState = "ON",
            int page = 1,
            int maxPrice = 0,
            int minBeds = 0,
            string sort = "relevance",
            CancellationToken cancellationToken = default)
        {
            var vm = new RentalListingsPageViewModel
            {
                Search = new RentalListingsSearchViewModel
                {
                    Query = query ?? string.Empty,
                    City = city ?? "Kitchener",
                    ProvinceOrState = provinceOrState ?? "ON",
                    Page = page < 1 ? 1 : page,
                    MaxPrice = maxPrice,
                    MinBeds = minBeds,
                    Sort = string.IsNullOrWhiteSpace(sort) ? "relevance" : sort
                }
            };

            try
            {
                vm.Listings = await _rentalListingsService.SearchRentalsAsync(vm.Search, cancellationToken);
                vm.StatusMessage = vm.Listings.Count == 0
                    ? "No listings found for this search."
                    : $"{vm.Listings.Count} listings found.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load rental listings.");
                vm.ErrorMessage = "Could not load listings right now. Check your RapidAPI configuration and try again.";
            }

            return View(vm);
        }
    }
}