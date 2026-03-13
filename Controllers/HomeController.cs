using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;
using System.Diagnostics;

namespace Shortlist.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        // authorize is used so error details are only shown to authenticate users.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
