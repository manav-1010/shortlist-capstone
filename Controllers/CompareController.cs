using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Helpers;
using Shortlist.Web.Models;
using System.Globalization;
using System.Text;

namespace Shortlist.Web.Controllers
{
    // lets a signed-in user manage their compare list stored in session:
    // Data is stored in Session
    [Authorize]
    public class CompareController : Controller
    {
        // Session key for storing compare list for current user session.
        private const string CompareSessionKey = "CompareItems";

        // compare page: loads the current session compare list.
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CompareIndexViewModel
            {
                Items = GetCompareItems()
            };
            return View(vm);
        }

        // adds a place to compare list, preventing duplicates and capping at 3 items.
        [HttpPost]
        public IActionResult Add([FromBody] CompareItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                return BadRequest("Missing item id.");

            var items = GetCompareItems();

            // prevent duplicates
            if (!items.Any(x => x.Id == item.Id))
            {
                // cap to 3
                if (items.Count >= 3)
                    return BadRequest("You can compare up to 3 places.");

                items.Add(item);
                SaveCompareItems(items);
            }

            return Ok(new { count = items.Count });
        }

        // removes one place from compare list (AJAX/JSON)
        [HttpPost]
        public IActionResult Remove([FromBody] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Missing id.");

            var items = GetCompareItems();
            items.RemoveAll(x => x.Id == id);
            SaveCompareItems(items);

            return Ok(new { count = items.Count });
        }

        // clears the entire compare list (AJAX/JSON)   
        [HttpPost]
        public IActionResult Clear()
        {
            SaveCompareItems(new List<CompareItem>());
            return Ok();
        }

        // exports the compare list as CSV file
        // uses invariant culture so decimals are consistent across machines
        [HttpGet]
        public IActionResult ExportCsv()
        {
            var items = GetCompareItems();
            if (items.Count == 0) return BadRequest("No compare items to export.");

            var sb = new StringBuilder();
            sb.AppendLine("Name,Category,DistanceKm,Lat,Lng,Id");

            foreach (var x in items)
            {
                sb.AppendLine(string.Join(",",
                    Csv(x.Name),
                    Csv(x.Category),
                    x.DistKm.ToString("0.00", CultureInfo.InvariantCulture),
                    x.Lat.ToString("0.000000", CultureInfo.InvariantCulture),
                    x.Lng.ToString("0.000000", CultureInfo.InvariantCulture),
                    Csv(x.Id)
                ));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "shortlist-compare.csv");
        }

        // ---------------- helpers ----------------
        // reads the compare list from JSON
        private List<CompareItem> GetCompareItems()
            => HttpContext.Session.GetJson<List<CompareItem>>(CompareSessionKey) ?? new List<CompareItem>();
        
        // writes the compare list back to session
        private void SaveCompareItems(List<CompareItem> items)
            => HttpContext.Session.SetJson(CompareSessionKey, items);

        // safe CSV escaping: wraps in quotes and doubles any internal quotes
        private static string Csv(string? s)
        {
            s ??= "";
            s = s.Replace("\"", "\"\"");
            return $"\"{s}\"";
        }
    }
}