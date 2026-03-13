using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortlist.Web.Data;
using Shortlist.Web.Models;
using System.Security.Claims;

namespace Shortlist.Web.Controllers
{
    // handles authentication for the app:
    // - Local login/register using Users table
    // - Google Login via OpenID Connect
    // - Cookie-based sign-in and session user id stroage
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        // Shows the login page
        // returnUrl is used to redirect back to the page the user was on after successful login
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = "/")
        {
            email = (email ?? "").Trim();
            password = password ?? "";

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return View();
            }

            await SignInUserAsync(user);

            return LocalRedirect(returnUrl);
        }
        // registration page, if already logged in, redirect to home page
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");

            return View();
        }
        // creates local account and signs in the user, then redirects to home page, if email already exists or passwords don't match, shows error message
        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string confirmPassword)
        {
            name = (name ?? "").Trim();
            email = (email ?? "").Trim();
            password = password ?? "";
            confirmPassword = confirmPassword ?? "";

            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                TempData["ErrorMessage"] = "An account with this email already exists.";
                return View();
            }

            var newUser = new UserProfile
            {
                Name = string.IsNullOrWhiteSpace(name) ? "User" : name,
                Email = email,
                Password = password
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            await SignInUserAsync(newUser);

            return RedirectToAction("Index", "Home");
        }

        // starts google sign in using OpenID Connect, after successful login, user is redirected back to the returnUrl or home page
        [HttpGet]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        // logs the user out by 
        // - clearing the session UserId
        // - removing the auth cookie
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserId");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // this is called when Google auth fails/cancels or access is denied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["ErrorMessage"] = "Login was cancelled or access was denied. Please try again.";
            return RedirectToAction("Login");
        }

        // Creates the user's ClaimsPrincipal and signs them in via cookie authentication.
        private async Task SignInUserAsync(UserProfile user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            // issue the auth cookie.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            HttpContext.Session.SetInt32("UserId", user.Id);
        }
    }
}