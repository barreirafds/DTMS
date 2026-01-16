using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DTMS.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/DashboardTables");
            }
            
            return Page();
        }
    }
}
