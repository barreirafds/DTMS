using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAcessLayer.Models;
using BusinessLogicLayer.Abstractions;

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
        public user User { get; set; } = new();

        public void OnGet()
        {
        }

        public ActionResult OnPost()
        {
            if (User.user1 == null || User.password == null)
            {
                ViewData["Message"] = "Invalid Credentials";
                return Page();
            }

            if (_authService.ValidateCredentials(User.user1, User.password))
            {
                return new RedirectToPageResult("Index");
            }
            else
            {
                ViewData["Message"] = "Invalid Credentials";
                return Page();
            }
        }
    }
}
