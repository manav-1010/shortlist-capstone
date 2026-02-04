using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            var state = GetFilterState();

            // Dummy results for skeleton (will replace with real ranking later)
            var results = new List<ResultItem>
            {
                new ResultItem { Name = "Area A", Score = 82, Pros = new(){"Close to campus"}, Cons = new(){"Higher rent"} },
                new ResultItem { Name = "Area B", Score = 75, Pros = new(){"Cheaper"}, Cons = new(){"Farther commute"} }
            };

            var vm = new ResultsViewModel
            {
                Filters = state,
                Items = results
            };

            return View(vm);
        }

        private FilterState GetFilterState()
        {
            var json = HttpContext.Session.GetString("FilterState");
            if (string.IsNullOrWhiteSpace(json)) return new FilterState();
            return JsonSerializer.Deserialize<FilterState>(json) ?? new FilterState();
        }
    }
}
