using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;

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

        public ActionResult OnPost()
        {
            var loginDto = new LoginDTO
            {
                Username = Username,
                Password = Password
            };

            var result = _authService.ValidateCredentials(loginDto);
            
            if (result.IsValid)
            {
                return new RedirectToPageResult("Index");
            }
            else
            {
                ViewData["Message"] = result.ErrorMessage ?? "Invalid Credentials";
                return Page();
            }
        }
    }
}
