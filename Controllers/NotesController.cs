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

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveNoteRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid note request.");

            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            var cleanStatus = NormalizeStatus(req.Status);
            var cleanText = string.IsNullOrWhiteSpace(req.NoteText) ? null : req.NoteText.Trim();
            var cleanTitle = string.IsNullOrWhiteSpace(req.Title) ? null : req.Title.Trim();

            var existing = await _db.UserNotes.FirstOrDefaultAsync(x =>
                x.UserId == userId.Value &&
                x.TargetType == req.TargetType &&
                x.TargetId == req.TargetId);

            if (existing == null)
            {
                existing = new UserNote
                {
                    UserId = userId.Value,
                    TargetType = req.TargetType.Trim(),
                    TargetId = req.TargetId.Trim(),
                };
                _db.UserNotes.Add(existing);
            }

            existing.Title = cleanTitle;
            existing.Status = cleanStatus;
            existing.NoteText = cleanText;
            existing.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                existing.Id,
                existing.Status,
                existing.NoteText,
                updatedAtUtc = existing.UpdatedAtUtc
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get(string targetType, string targetId)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            var note = await _db.UserNotes
                .Where(x => x.UserId == userId.Value && x.TargetType == targetType && x.TargetId == targetId)
                .Select(x => new NoteCardDto
                {
                    TargetType = x.TargetType,
                    TargetId = x.TargetId,
                    Title = x.Title,
                    Status = x.Status,
                    NoteText = x.NoteText,
                    UpdatedAtUtc = x.UpdatedAtUtc
                })
                .FirstOrDefaultAsync();

            return Json(note);
        }

        [HttpGet]
        public async Task<IActionResult> Bulk(string targetType, [FromQuery] string[] ids)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            ids ??= Array.Empty<string>();

            var notes = await _db.UserNotes
                .Where(x => x.UserId == userId.Value && x.TargetType == targetType && ids.Contains(x.TargetId))
                .Select(x => new NoteCardDto
                {
                    TargetType = x.TargetType,
                    TargetId = x.TargetId,
                    Title = x.Title,
                    Status = x.Status,
                    NoteText = x.NoteText,
                    UpdatedAtUtc = x.UpdatedAtUtc
                })
                .ToListAsync();

            return Json(notes);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] SaveNoteRequest req)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            var existing = await _db.UserNotes.FirstOrDefaultAsync(x =>
                x.UserId == userId.Value &&
                x.TargetType == req.TargetType &&
                x.TargetId == req.TargetId);

            if (existing == null)
                return Json(new { ok = true });

            _db.UserNotes.Remove(existing);
            await _db.SaveChangesAsync();

            return Json(new { ok = true });
        }

        [HttpGet]
        public async Task<IActionResult> Recent(int count = 5)
        {
            var userId = await EnsureUserIdAsync();
            if (userId == null) return Unauthorized();

            count = Math.Clamp(count, 1, 12);

            var notes = await _db.UserNotes
                .Where(x => x.UserId == userId.Value)
                .OrderByDescending(x => x.UpdatedAtUtc)
                .Take(count)
                .Select(x => new NoteCardDto
                {
                    TargetType = x.TargetType,
                    TargetId = x.TargetId,
                    Title = x.Title,
                    Status = x.Status,
                    NoteText = x.NoteText,
                    UpdatedAtUtc = x.UpdatedAtUtc
                })
                .ToListAsync();

            return Json(notes);
        }

        private static string NormalizeStatus(string? status)
        {
            var s = (status ?? "").Trim();

            return s switch
            {
                "Interested" => "Interested",
                "Not for me" => "Not for me",
                _ => "Maybe"
            };
        }

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