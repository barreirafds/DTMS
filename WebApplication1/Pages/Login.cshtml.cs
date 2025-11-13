using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;

// AUTHENTICATION USING

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DTMS.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var loginDto = new LoginDTO
            {
                Username = Username,
                Password = Password
            };

            var result = _authService.ValidateCredentials(loginDto);

            if (result.IsValid)
            {
                // cria identidade do utilizador
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, Username)
                };

                var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);

                return RedirectToPage("Index");
            }
            else
            {
                ViewData["Message"] = result.ErrorMessage ?? "Invalid Credentials";
                return Page();
            }
        }
    }
}
