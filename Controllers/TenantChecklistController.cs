using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class TenantChecklistController : Controller
    {
        // displays tenant checklist page.
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}