using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics.Eventing.Reader;
using DTMS.Data.Models;

namespace DTMS.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]

        public user User { get; set; }
        public void OnGet()
        {
        }

        public ActionResult OnPost()
        {
            if (User.user1 == "user" && User.password=="password")
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
