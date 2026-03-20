using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Data;
using Shortlist.Web.Models;
using System.Security.Claims;

namespace Shortlist.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly AppDbContext _db;

        public NotesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var notes = await _db.AreaNotes
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.UpdatedAtUtc)
                .ToListAsync();

            return View(notes);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                return BadRequest("PlaceId is required.");
            }

            var userId = await EnsureUserIdAsync();
            if (userId == null)
            {
                return Unauthorized();
            }

            var note = await _db.AreaNotes
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.UserId == userId.Value && n.PlaceId == placeId);

            if (note == null)
            {
                return Json(new
                {
                    exists = false,
                    placeId,
                    noteText = "",
                    placeName = "",
                    category = ""
                });
            }

            return Json(new
            {
                exists = true,
                id = note.Id,
                placeId = note.PlaceId,
                placeName = note.PlaceName,
                category = note.Category,
                lat = note.Lat,
                lng = note.Lng,
                noteText = note.NoteText,
                createdAtUtc = note.CreatedAtUtc,
                updatedAtUtc = note.UpdatedAtUtc
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(NoteUpsertViewModel vm)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(vm.PlaceId))
            {
                return BadRequest("PlaceId is required.");
            }

            if (string.IsNullOrWhiteSpace(vm.PlaceName))
            {
                return BadRequest("PlaceName is required.");
            }

            var noteText = (vm.NoteText ?? string.Empty).Trim();

            if (noteText.Length > 2000)
            {
                return BadRequest("Note must be 2000 characters or less.");
            }

            var existing = await _db.AreaNotes
                .FirstOrDefaultAsync(n => n.UserId == userId.Value && n.PlaceId == vm.PlaceId);

            if (existing == null)
            {
                var newNote = new AreaNote
                {
                    UserId = userId.Value,
                    PlaceId = vm.PlaceId.Trim(),
                    PlaceName = vm.PlaceName.Trim(),
                    Category = string.IsNullOrWhiteSpace(vm.Category) ? null : vm.Category.Trim(),
                    Lat = vm.Lat,
                    Lng = vm.Lng,
                    NoteText = noteText
                };

                _db.AreaNotes.Add(newNote);
                await _db.SaveChangesAsync();

                return Json(new
                {
                    ok = true,
                    message = "Note saved.",
                    id = newNote.Id
                });
            }

            existing.PlaceName = vm.PlaceName.Trim();
            existing.Category = string.IsNullOrWhiteSpace(vm.Category) ? null : vm.Category.Trim();
            existing.Lat = vm.Lat;
            existing.Lng = vm.Lng;
            existing.NoteText = noteText;

            await _db.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                message = "Note updated.",
                id = existing.Id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null)
            {
                return Unauthorized();
            }

            var note = await _db.AreaNotes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.Value);

            if (note == null)
            {
                return NotFound("Note not found.");
            }

            _db.AreaNotes.Remove(note);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Note deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteByPlaceId(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                return BadRequest("PlaceId is required.");
            }

            var userId = await EnsureUserIdAsync();
            if (userId == null)
            {
                return Unauthorized();
            }

            var note = await _db.AreaNotes
                .FirstOrDefaultAsync(n => n.UserId == userId.Value && n.PlaceId == placeId);

            if (note == null)
            {
                return NotFound("Note not found.");
            }

            _db.AreaNotes.Remove(note);
            await _db.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                message = "Note deleted."
            });
        }

        private async Task<int?> EnsureUserIdAsync()
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserId");
            if (userIdFromSession.HasValue)
            {
                return userIdFromSession.Value;
            }

            var email =
                User.FindFirstValue(ClaimTypes.Email) ??
                User.FindFirstValue("email") ??
                User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            email = email.Trim().ToLowerInvariant();

            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (existing != null)
            {
                HttpContext.Session.SetInt32("UserId", existing.Id);
                return existing.Id;
            }

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
    }
}