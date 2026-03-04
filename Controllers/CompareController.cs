using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Helpers;
using Shortlist.Web.Models;
using System.Globalization;
using System.Text;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class CompareController : Controller
    {
        private const string CompareSessionKey = "CompareItems";

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CompareIndexViewModel
            {
                Items = GetCompareItems()
            };
            return View(vm);
        }

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

        [HttpPost]
        public IActionResult Clear()
        {
            SaveCompareItems(new List<CompareItem>());
            return Ok();
        }

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
        private List<CompareItem> GetCompareItems()
            => HttpContext.Session.GetJson<List<CompareItem>>(CompareSessionKey) ?? new List<CompareItem>();

        private void SaveCompareItems(List<CompareItem> items)
            => HttpContext.Session.SetJson(CompareSessionKey, items);

        private static string Csv(string? s)
        {
            s ??= "";
            s = s.Replace("\"", "\"\"");
            return $"\"{s}\"";
        }
    }
}