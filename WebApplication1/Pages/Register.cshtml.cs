using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;

namespace DTMS.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;

        public RegisterModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please confirm your password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please select a role")]
        public string Role { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var registerDto = new RegisterDTO
            {
                Username = Username,
                Password = Password,
                ConfirmPassword = ConfirmPassword,
                Role = Role
            };

            var result = _authService.RegisterUser(registerDto);
            
            if (result.IsValid)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                ModelState.AddModelError(result.FieldName ?? "", result.ErrorMessage ?? "Registration failed.");
                return Page();
            }
        }
    }
}

