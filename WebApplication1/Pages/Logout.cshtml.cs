using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = "/"
        };

        return SignOut(
            props,
            CookieAuthenticationDefaults.AuthenticationScheme,
            Auth0Constants.AuthenticationScheme
        );
    }
}
