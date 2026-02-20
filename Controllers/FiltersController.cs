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
            var state = GetFilterState();
            return View(state);
        }

        [HttpPost]
        public IActionResult Index(FilterState state)
        {
            // Enforce max 3 priorities server-side too
            state.Priorities = (state.Priorities ?? new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();

            // If validation fails, re-render page with errors + preserve selections
            if (!ModelState.IsValid)
            {
                return View(state);
            }

            SaveFilterState(state);
            return RedirectToAction("Index", "Results");
        }

        [HttpPost]
        public IActionResult Reset()
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

        private void SaveFilterState(FilterState state)
        {
            var json = JsonSerializer.Serialize(state);
            HttpContext.Session.SetString("FilterState", json);
        }
    }
}