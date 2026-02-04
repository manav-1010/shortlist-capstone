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
            // Load from session if present, else default
            var state = GetFilterState();
            return View(state);
        }

        // Nirali will implement apply behavior 
        [HttpPost]
        public IActionResult Index(FilterState state)
        {
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
