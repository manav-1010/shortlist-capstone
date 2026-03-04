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
            if (userId == null) return Unauthorized();

            var state = GetFilterState();
            if (state == null) return BadRequest("No filter state in session.");

            if (string.IsNullOrWhiteSpace(name))
            {
                name = BuildAutoName(state);
            }

            name = name.Trim();

            if (name.Length > 60)
                name = name[..60]; if (name.Length > 60) name = name[..60];

            var entity = new SavedSearch
            {
                UserId = userId.Value,
                Name = name,
                FilterStateJson = JsonSerializer.Serialize(state),
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.SavedSearches.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Saved" });
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
            // 1) Session first (fast + stable)
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession.HasValue) return userIdFromSession.Value;

            // 2) Get email from claims (Google + cookie auth)
            var email =
                User.FindFirstValue(ClaimTypes.Email) ??
                User.FindFirstValue("email") ??
                User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email)) return null;

            email = email.Trim().ToLowerInvariant();

            // 3) Find existing user
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (existing != null)
            {
                HttpContext.Session.SetInt32("UserId", existing.Id);
                return existing.Id;
            }

            // 4) Create user (Google user won't have password)
            var newUser = new UserProfile
            {
                Email = email,
                Password = Guid.NewGuid().ToString("N") // placeholder; not used for Google
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
        private static string BuildAutoName(FilterState state)
        {
            var parts = new List<string>();

            if (state.Budget.HasValue && state.Budget.Value > 0)
                parts.Add($"Budget ${state.Budget.Value:0}");

            // Use RadiusKm (your search radius)
            if (state.RadiusKm > 0)
                parts.Add($"{state.RadiusKm}km");

            // Optional nicer label if you store it
            if (!string.IsNullOrWhiteSpace(state.LocationLabel))
                parts.Add(state.LocationLabel.Trim());

            if (state.Priorities != null && state.Priorities.Any())
                parts.Add(string.Join("+", state.Priorities.Take(2)));

            // ✅ MUST return something on all paths
            return parts.Count > 0 ? string.Join(" • ", parts) : "Saved Search";
        }
        [AllowAnonymous]
        [HttpGet("/SavedSearches/Share/{token:guid}")]
        public async Task<IActionResult> Share(Guid token)
        {
            var saved = await _db.SavedSearches
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShareToken == token);

            if (saved == null)
                return NotFound("Share link is invalid or expired.");

            var state = JsonSerializer.Deserialize<FilterState>(saved.FilterStateJson);
            if (state == null)
                return BadRequest("Saved search is corrupted.");

            SaveFilterState(state);

            // Optional: small toast for user experience
            TempData["Toast"] = $"Loaded shared search: {saved.Name}";
            return RedirectToAction("Index", "Results");
        }
        [HttpPost]
        public async Task<IActionResult> RegenerateShareLink(int id)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            var saved = await _db.SavedSearches
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId.Value);

            if (saved == null) return NotFound();

            saved.ShareToken = Guid.NewGuid();
            await _db.SaveChangesAsync();

            return Ok(new { token = saved.ShareToken });
        }
    }
}