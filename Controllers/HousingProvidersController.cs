using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Web.Models;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class HousingProvidersController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? q)
        {
            q = (q ?? "").Trim().ToLowerInvariant();

            // Real links (no mock), curated list
            var providers = new List<HousingProvider>
            {
                new HousingProvider {
                    Name = "Accommod8u",
                    City = "Kitchener–Waterloo",
                    WebsiteUrl = "https://www.accommod8u.com/",
                    Tags = "student housing, apartments, waterloo",
                    Notes = "Popular student rentals near UW/Laurier. Check availability early."
                },
                new HousingProvider {
                    Name = "KW4Rent",
                    City = "Kitchener–Waterloo",
                    WebsiteUrl = "https://kw4rent.com/",
                    Tags = "property management, rentals, kw",
                    Notes = "Local property management listings across KW."
                },
                new HousingProvider {
                    Name = "Domus",
                    City = "Waterloo",
                    WebsiteUrl = "https://domusinc.ca/",
                    Tags = "student housing, waterloo",
                    Notes = "Student-focused housing options in Waterloo."
                },
                new HousingProvider {
                    Name = "Sage Living",
                    City = "Waterloo",
                    WebsiteUrl = "https://sageliving.ca/",
                    Tags = "student housing, waterloo",
                    Notes = "Student residences close to campus (availability varies by term)."
                },
                new HousingProvider {
                    Name = "Drewlo Holdings",
                    City = "Kitchener–Waterloo",
                    WebsiteUrl = "https://www.drewloholdings.com/",
                    Tags = "apartments, rentals, property management",
                    Notes = "Purpose-built rental apartments (good for non-student options too)."
                },
                new HousingProvider {
                    Name = "Rentals.ca (KW listings)",
                    City = "Kitchener–Waterloo",
                    WebsiteUrl = "https://rentals.ca/kitchener",
                    Tags = "marketplace, listings, kw",
                    Notes = "Marketplace aggregator for browsing live listings quickly."
                }
            };

            if (!string.IsNullOrWhiteSpace(q))
            {
                providers = providers
                    .Where(p =>
                        (p.Name ?? "").ToLowerInvariant().Contains(q) ||
                        (p.City ?? "").ToLowerInvariant().Contains(q) ||
                        (p.Tags ?? "").ToLowerInvariant().Contains(q) ||
                        (p.Notes ?? "").ToLowerInvariant().Contains(q)
                    )
                    .ToList();
            }

            ViewBag.Query = q;
            ViewBag.Total = providers.Count;
            return View(providers);
        }
    }
}