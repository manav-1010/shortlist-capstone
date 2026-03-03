using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Data;
using Shortlist.Web.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class SavedSearchesController : Controller
    {
        private readonly AppDbContext _db;

        public SavedSearchesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return RedirectToAction("Login", "Account");

            var items = await _db.SavedSearches
                .Where(s => s.UserId == userId.Value)
                .OrderByDescending(s => s.CreatedAtUtc)
                .ToListAsync();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCurrent(string name)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return RedirectToAction("Login", "Account");

            var state = GetFilterState();
            if (state == null) return RedirectToAction("Index", "Filters");

            name = string.IsNullOrWhiteSpace(name) ? "Saved Search" : name.Trim();
            if (name.Length > 60) name = name[..60];

            var entity = new SavedSearch
            {
                UserId = userId.Value,
                Name = name,
                FilterStateJson = JsonSerializer.Serialize(state),
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.SavedSearches.Add(entity);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Saved!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Load(int id)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return RedirectToAction("Login", "Account");

            var saved = await _db.SavedSearches
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId.Value);

            if (saved == null) return NotFound();

            var state = JsonSerializer.Deserialize<FilterState>(saved.FilterStateJson);
            if (state == null) return RedirectToAction("Index", "Filters");

            SaveFilterState(state);
            return RedirectToAction("Index", "Results");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return RedirectToAction("Login", "Account");

            var saved = await _db.SavedSearches
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId.Value);

            if (saved == null) return NotFound();

            _db.SavedSearches.Remove(saved);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Deleted.";
            return RedirectToAction("Index");
        }

        // ---------- helpers ----------

        private async Task<int?> EnsureUserIdAsync()
        {
            // 1) Try claim NameIdentifier (works for email/password login)
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var userIdFromClaim))
            {
                HttpContext.Session.SetInt32("UserId", userIdFromClaim);
                return userIdFromClaim;
            }

            // 2) Try session (works once we set it)
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession.HasValue) return userIdFromSession.Value;

            // 3) Fallback: use Email claim (works for Google)
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(email)) return null;

            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existing != null)
            {
                HttpContext.Session.SetInt32("UserId", existing.Id);
                return existing.Id;
            }

            // 4) If Google user doesn't exist in our DB, create it (simple + works)
            var displayName = User.Identity?.Name ?? email;

            var newUser = new User
            {
                Name = displayName,
                Email = email,
                Password = Guid.NewGuid().ToString("N") // random; they won't use it
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", newUser.Id);
            return newUser.Id;
        }

        private FilterState? GetFilterState()
        {
            var json = HttpContext.Session.GetString("FilterState");
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonSerializer.Deserialize<FilterState>(json);
        }

        private void SaveFilterState(FilterState state)
        {
            var json = JsonSerializer.Serialize(state);
            HttpContext.Session.SetString("FilterState", json);
        }
    }
}