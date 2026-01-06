using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace DTMS.Pages
{
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGet(string returnUrl = "/")
        {
            try
            {
                var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                    .WithRedirectUri(returnUrl)
                    .Build();

                await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                // Log error if needed
                TempData["ErrorMessage"] = $"An error occurred during login: {ex.Message}";
                return RedirectToPage("/Login");
            }
        }
    }
}
