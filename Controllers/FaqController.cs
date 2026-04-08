using Microsoft.AspNetCore.Mvc;

namespace Shortlist.Web.Controllers
{
    public class FaqController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}