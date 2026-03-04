using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    [IgnoreAntiforgeryToken] // because we're calling via fetch() from JS
    public class CompareController : Controller
    {
        private const string SessionKey = "CompareItems";
        private const int MaxItems = 3;

        [HttpGet]
        public IActionResult Index()
        {
            var items = GetCompareItems();
            var vm = new CompareIndexViewModel { Items = items };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Add([FromBody] CompareItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                return BadRequest("Invalid compare item.");

            var items = GetCompareItems();

            // Prevent duplicates
            if (!items.Any(x => x.Id == item.Id))
            {
                // Max 3
                if (items.Count >= MaxItems)
                    return BadRequest($"You can compare up to {MaxItems} places.");

                // Clean fields
                item.Name = string.IsNullOrWhiteSpace(item.Name) ? "Unnamed place" : item.Name.Trim();
                item.Category = string.IsNullOrWhiteSpace(item.Category) ? "OTHER" : item.Category.Trim();

                items.Add(item);
                SaveCompareItems(items);
            }

            return Ok(new { count = items.Count });
        }

        [HttpPost]
        public IActionResult Remove([FromBody] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("Missing id.");

            var items = GetCompareItems();
            items.RemoveAll(x => x.Id == id);
            SaveCompareItems(items);

            return Ok(new { count = items.Count });
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(SessionKey);
            return Ok(new { count = 0 });
        }

        [HttpGet]
        public IActionResult Count()
        {
            var items = GetCompareItems();
            return Ok(new { count = items.Count });
        }

        // -------- helpers --------

        private List<CompareItem> GetCompareItems()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(json)) return new List<CompareItem>();

            try
            {
                return JsonSerializer.Deserialize<List<CompareItem>>(json) ?? new List<CompareItem>();
            }
            catch
            {
                return new List<CompareItem>();
            }
        }

        private void SaveCompareItems(List<CompareItem> items)
        {
            var json = JsonSerializer.Serialize(items);
            HttpContext.Session.SetString(SessionKey, json);
        }
    }
}