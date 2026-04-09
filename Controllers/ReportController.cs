using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    public class ReportController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var isPremium = HttpContext.Session.GetString("IsPremium") == "true";
            if (!isPremium)
                return RedirectToAction("Index", "Premium");

            var json = TempData["AiReport"] as string;
            if (string.IsNullOrWhiteSpace(json))
            {
                TempData["Toast"] = "No AI report data found. Please generate the report again.";
                return RedirectToAction("Index", "Results");
            }

            var vm = JsonSerializer.Deserialize<AiRecommendationReportViewModel>(json)
                     ?? new AiRecommendationReportViewModel();

            return View(vm);
        }

        [HttpPost]
        public IActionResult Generate([FromBody] AiRecommendationReportViewModel model)
        {
            var isPremium = HttpContext.Session.GetString("IsPremium") == "true";
            if (!isPremium)
                return Unauthorized("Premium access required.");

            if (model == null || string.IsNullOrWhiteSpace(model.AreaName))
                return BadRequest("Invalid report data.");

            model.GeneratedAt = DateTime.Now;

            TempData["AiReport"] = JsonSerializer.Serialize(model);

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index", "Report")
            });
        }
    }
}