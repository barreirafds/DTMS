using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DTMS.Data;
using DTMS.Data.Models;

namespace DTMS.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public List<table> ShowTableEmployee { get; private set; } = new();

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ShowTableEmployee = new tableconn().GetTables();
        }

        public string showtables()
        {
            tableconn tables2 = new tableconn();
            return tables2.connString;
        }

        // Function to get  the badge (color bg) of the status
        public string GetStatusBadgeClass(string status)
        {
            var s = (status ?? "Available").ToLowerInvariant();
            
            if (s == "available") return "bg-success";
            else if (s == "occupied") return "bg-danger";
            else if (s == "reserved") return "bg-warning";
            else return "bg-secondary";
        }
    }
}
