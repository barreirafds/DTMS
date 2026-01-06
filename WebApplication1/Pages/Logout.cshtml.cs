using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;

namespace DTMS.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                    .WithRedirectUri("/")
                    .Build();

                await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                // Log error if needed
                TempData["ErrorMessage"] = $"An error occurred during logout: {ex.Message}";
                return RedirectToPage("/");
            }
        }
    }
}

