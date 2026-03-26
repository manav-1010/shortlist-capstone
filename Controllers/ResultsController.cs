using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    public class ResultsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var state = GetFilterState();

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

        private FilterState GetFilterState()
        {
            var json = HttpContext.Session.GetString("FilterState");
            if (string.IsNullOrWhiteSpace(json)) return new FilterState();
            return JsonSerializer.Deserialize<FilterState>(json) ?? new FilterState();
        }
    }
}