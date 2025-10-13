using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics.Eventing.Reader;
using DTMS.Models;

namespace DTMS.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]

        public User User { get; set; }
        public void OnGet()
        {
        }

        public ActionResult OnPost()
        {
            if (User.Username == "user" && User.Password=="password")
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
