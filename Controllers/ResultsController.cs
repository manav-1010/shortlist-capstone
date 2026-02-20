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

            // Dummy results for skeleton (Sprint 1)
            var chosen = state.Priorities ?? new List<string>();
            var chosenText = chosen.Count > 0 ? string.Join(", ", chosen) : "None";

            var results = new List<ResultItem>
            {
                new ResultItem
                {
                    Name = "Area A",
                    Score = 82,
                    Pros = new() { "Close to campus", $"Matched priorities: {chosenText}" },
                    Cons = new() { "Higher rent" }
                },
                new ResultItem
                {
                    Name = "Area B",
                    Score = 75,
                    Pros = new() { "Cheaper", $"Matched priorities: {chosenText}" },
                    Cons = new() { "Farther commute" }
                }
            };

            // ✅ Edge case for Testing Assignment:
            // If Budget is extremely low, show empty results (to test "No results" state).
            if (state.Budget.HasValue && state.Budget.Value < 100)
            {
                results = new List<ResultItem>();
            }

            var vm = new ResultsViewModel
            {
                Filters = state,
                Items = results
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