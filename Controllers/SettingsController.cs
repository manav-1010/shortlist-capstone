using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Data;
using Shortlist.Web.Models;
using System.Security.Claims;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly AppDbContext _db;

        public SettingsController(AppDbContext db)
        {
            _db = db;
        }

        // loads the current user + their saved preferences from the database.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // resolve current user's DB id
            var userId = await EnsureUserIdAsync();
            if (userId == null) return RedirectToAction("Login", "Account");

            // include settings navigation property 
            var user = await _db.Users
                .Include(u => u.Settings)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null) return RedirectToAction("Login", "Account");

            // detect provider 
            var provider = User.FindFirstValue("iss") != null || User.FindFirstValue("urn:google:picture") != null
                ? "Google"
                : "Local";

            // build view model with safe defaults
            var vm = new SettingsViewModel
            {
                Email = user.Email,
                AuthProvider = provider,
                DefaultRadiusKm = user.Settings?.DefaultRadiusKm ?? 3,
                DefaultPriorities = user.Settings?.DefaultPrioritiesCsv ?? "",
                DefaultLocationLabel = user.Settings?.DefaultLocationLabel
            };

            return View(vm);
        }

        // persists user preferences to the database
        // uses antiforgery for standard form protection.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SettingsViewModel vm)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            // parse priorities → max 3
            var tokens = (vm.DefaultPriorities ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.ToLowerInvariant())
                .Distinct()
                .Take(3)
                .ToList();

            var csv = string.Join(",", tokens);

            // upsert: create settings row it it doesn't exist yet.
            var settings = await _db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId.Value);
            if (settings == null)
            {
                settings = new UserSettings { UserId = userId.Value };
                _db.UserSettings.Add(settings);
            }

            // save preferences
            settings.DefaultRadiusKm = vm.DefaultRadiusKm;
            settings.DefaultPrioritiesCsv = csv;
            settings.DefaultLocationLabel = string.IsNullOrWhiteSpace(vm.DefaultLocationLabel) ? null : vm.DefaultLocationLabel.Trim();

            await _db.SaveChangesAsync();

            TempData["Toast"] = "Settings saved.";
            return RedirectToAction("Index");
        }

        // Data controls

        // clears current session.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Remove("FilterState");
            TempData["Toast"] = "Session filters cleared.";
            return RedirectToAction("Index");
        }

        // clears compare list from session.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCompare()
        {
            HttpContext.Session.Remove("CompareItems");
            TempData["Toast"] = "Compare list cleared.";
            return RedirectToAction("Index");
        }

        // Deletes all saved searches for current user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllSaved()
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            var items = await _db.SavedSearches.Where(s => s.UserId == userId.Value).ToListAsync();
            _db.SavedSearches.RemoveRange(items);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "All saved searches deleted.";
            return RedirectToAction("Index");
        }

       // ---------- User identity helper ----------

        // resolve current user's UserProfile ID.
        private async Task<int?> EnsureUserIdAsync()
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession.HasValue) return userIdFromSession.Value;

            var email =
                User.FindFirstValue(ClaimTypes.Email) ??
                User.FindFirstValue("email") ??
                User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email)) return null;

            email = email.Trim().ToLowerInvariant();

            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (existing != null)
            {
                HttpContext.Session.SetInt32("UserId", existing.Id);
                return existing.Id;
            }

            // for google maps, we create a minimal local profile to save preferences.
            var newUser = new UserProfile
            {
                Email = email,
                Password = Guid.NewGuid().ToString("N")
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", newUser.Id);
            return newUser.Id;
        }

        // Legal page (Privacy Policy / Terms / Credits)
        // Accessible from Settings 
        [HttpGet]
        public IActionResult Legal()
        {
            return View();
        }
    }
}