using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;

namespace Shortlist.Web.Controllers
{
    public class PremiumController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.IsPremium = HttpContext.Session.GetString("IsPremium") == "true";
            ViewBag.PremiumPlan = HttpContext.Session.GetString("PremiumPlan") ?? "";
            ViewBag.PremiumEndDate = HttpContext.Session.GetString("PremiumEndDate") ?? "";
            return View();
        }

        [HttpGet]
        public IActionResult Checkout(string plan = "monthly")
        {
            var vm = new PremiumCheckoutViewModel
            {
                Plan = string.IsNullOrWhiteSpace(plan) ? "monthly" : plan.ToLower()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartTrial(PremiumCheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            // Demo payment success + 1 month free trial
            var now = DateTime.UtcNow;
            var end = now.AddMonths(1);

            HttpContext.Session.SetString("IsPremium", "true");
            HttpContext.Session.SetString("PremiumPlan", model.Plan ?? "monthly");
            HttpContext.Session.SetString("PremiumStartDate", now.ToString("O"));
            HttpContext.Session.SetString("PremiumEndDate", end.ToString("O"));

            TempData["Toast"] = "Premium trial activated successfully!";
            return RedirectToAction("Index", "Results");
        }
    }
}