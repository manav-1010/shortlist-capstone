using Microsoft.AspNetCore.Mvc;

namespace Shortlist.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Ty's part (hardcoded for Sprint 1 is fine)
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
