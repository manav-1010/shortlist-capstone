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

            var vm = new ResultsViewModel
            {
                Filters = state,
                Items = new List<ResultItem>() // will be filled by JS in real-time
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