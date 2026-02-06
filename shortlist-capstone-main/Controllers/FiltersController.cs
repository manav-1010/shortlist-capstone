using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    public class FiltersController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // load last saved filters (if any) so the form stays filled
            var state = GetFilterState();
            return View(state);
        }

        [HttpPost]
        public IActionResult Index(FilterState state)
        {
            // quick server-side safety check (UI blocks it too): max 3 priorities
            state.Priorities = (state.Priorities ?? new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();

            // save filters so Results page can read them
            SaveFilterState(state);
            return RedirectToAction("Index", "Results");
        }

        private FilterState GetFilterState()
        {
            var json = HttpContext.Session.GetString("FilterState");
            if (string.IsNullOrWhiteSpace(json)) return new FilterState();
            return JsonSerializer.Deserialize<FilterState>(json) ?? new FilterState();
        }

        private void SaveFilterState(FilterState state)
        {
            var json = JsonSerializer.Serialize(state);
            HttpContext.Session.SetString("FilterState", json);
        }
    }
}
