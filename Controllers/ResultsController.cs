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
            // Read the most recent filters selected by the user
            var state = GetFilterState();

            // viewmodel contains filters and an initially empty list of results
            var vm = new ResultsViewModel
            {
                Filters = state,
                Items = new List<ResultItem>() // will be filled by JS in real-time
            };

            return View(vm);
        }
        // clears the current filter state and reloads the Results page.
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("FilterState");
            return RedirectToAction("Index");
        }

        // Reads FilterState JSON from Session.
        private FilterState GetFilterState()
        {
            var json = HttpContext.Session.GetString("FilterState");
            if (string.IsNullOrWhiteSpace(json)) return new FilterState();
            return JsonSerializer.Deserialize<FilterState>(json) ?? new FilterState();
        }
    }
}