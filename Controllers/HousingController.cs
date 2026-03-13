using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class HousingController : Controller
    {
        // displays a list of housing providers.
        [HttpGet]
        public IActionResult Index(string? q)
        {
            var providers = GetProviders();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLowerInvariant();
                providers = providers
                    .Where(p =>
                        (p.Name ?? "").ToLower().Contains(q) ||
                        (p.City ?? "").ToLower().Contains(q) ||
                        (p.Tags ?? "").ToLower().Contains(q) ||
                        (p.Notes ?? "").ToLower().Contains(q))
                    .ToList();
            }

            ViewBag.Query = q ?? "";
            return View(providers);
        }

        // returns a curated list of housing providers commonly used by students.
        private static List<HousingProvider> GetProviders() => new()
        {
            new HousingProvider { Name="Accommod8u", City="Waterloo", Tags="student rentals, apartments", Url="https://accommod8u.com/", Notes="Popular student rentals in Waterloo/Kitchener." },
            new HousingProvider { Name="Domus", City="Waterloo", Tags="student housing", Url="https://domusinc.ca/", Notes="Student-focused properties near UW/WLU." },
            new HousingProvider { Name="WOCH (Waterloo Off-Campus Housing)", City="Waterloo", Tags="university listings", Url="https://listings.och.uwaterloo.ca/", Notes="Official off-campus listing board (UW)." },
            new HousingProvider { Name="Places4Students", City="Waterloo / Kitchener", Tags="student listings", Url="https://www.places4students.com/", Notes="Student housing search across cities." },
            new HousingProvider { Name="Kijiji Rentals", City="Canada", Tags="marketplace", Url="https://www.kijiji.ca/b-apartments-condos/canada/c37l0", Notes="Good for deals; verify listings carefully." },
            new HousingProvider { Name="Facebook Marketplace", City="Canada", Tags="marketplace", Url="https://www.facebook.com/marketplace/category/propertyrentals", Notes="Fast-moving listings; watch for scams." },
            new HousingProvider { Name="Realtor.ca Rentals", City="Canada", Tags="realtor listings", Url="https://www.realtor.ca/", Notes="More formal rentals; condos & managed units." },
            new HousingProvider { Name="Rentals.ca", City="Canada", Tags="rental search", Url="https://rentals.ca/", Notes="Search by city + filters." },
            new HousingProvider { Name="PadMapper", City="Canada", Tags="aggregator", Url="https://www.padmapper.com/", Notes="Aggregates listings; good map-based browsing." }
        };
    }
}