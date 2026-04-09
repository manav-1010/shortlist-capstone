using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using Shortlist.Web.Services;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    public class ResultsController : Controller
    {
        private readonly OllamaRecommendationService _ollamaRecommendationService;

        public ResultsController(OllamaRecommendationService ollamaRecommendationService)
        {
            _ollamaRecommendationService = ollamaRecommendationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var state = GetFilterState();
            if (state.Lat is null || state.Lng is null)
            {
                return RedirectToAction("Index", "Filters");
            }

            var isPremium = HttpContext.Session.GetString("IsPremium") == "true";
            var premiumEndText = HttpContext.Session.GetString("PremiumEndDate");

            if (!string.IsNullOrWhiteSpace(premiumEndText) &&
                DateTime.TryParse(premiumEndText, out var premiumEndUtc))
            {
                if (DateTime.UtcNow > premiumEndUtc)
                {
                    HttpContext.Session.Remove("IsPremium");
                    HttpContext.Session.Remove("PremiumPlan");
                    HttpContext.Session.Remove("PremiumStartDate");
                    HttpContext.Session.Remove("PremiumEndDate");
                    isPremium = false;
                }
            }

            ViewBag.IsPremium = isPremium;

            var vm = new ResultsViewModel
            {
                Filters = state,
                Items = new List<ResultItem>()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("FilterState");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateBestAreaExplanation([FromBody] BestAreaRequest request)
        {
            var isPremium = HttpContext.Session.GetString("IsPremium") == "true";
            if (!isPremium)
                return Unauthorized("Premium access required.");

            if (request == null || string.IsNullOrWhiteSpace(request.AreaName))
                return BadRequest("Invalid recommendation request.");

            var result = await _ollamaRecommendationService.GenerateBestAreaSummaryAsync(
                request.AreaName,
                request.Score,
                request.Priorities ?? new List<string>(),
                request.MatchCount,
                request.AvgDistanceKm);

            return Json(result);
        }

        private FilterState GetFilterState()
        {
            var json = HttpContext.Session.GetString("FilterState");
            if (string.IsNullOrWhiteSpace(json)) return new FilterState();
            return JsonSerializer.Deserialize<FilterState>(json) ?? new FilterState();
        }
    }

    public class BestAreaRequest
    {
        public string AreaName { get; set; } = "";
        public int Score { get; set; }
        public List<string> Priorities { get; set; } = new();
        public int MatchCount { get; set; }
        public double AvgDistanceKm { get; set; }
    }
}