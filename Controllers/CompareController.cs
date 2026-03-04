using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    public class CompareController : Controller
    {
        private const string SessionKey = "CompareList";

        public IActionResult Index()
        {
            var json = HttpContext.Session.GetString(SessionKey);

            if (string.IsNullOrEmpty(json))
                return View(new List<int>());

            var list = JsonSerializer.Deserialize<List<int>>(json);

            return View(list);
        }

        [HttpPost]
        public IActionResult Add(int id)
        {
            var json = HttpContext.Session.GetString(SessionKey);
            var list = string.IsNullOrEmpty(json)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(json);

            if (!list.Contains(id) && list.Count < 3)
                list.Add(id);

            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(list));

            return Ok();
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var json = HttpContext.Session.GetString(SessionKey);

            if (string.IsNullOrEmpty(json))
                return Ok();

            var list = JsonSerializer.Deserialize<List<int>>(json);

            list.Remove(id);

            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(list));

            return Ok();
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction("Index");
        }
    }
}