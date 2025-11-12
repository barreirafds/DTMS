using BusinessLogicLayer.Abstractions;
using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DTMS.Pages
{
    public class EditTableModel : PageModel
    {
        private readonly ITableService _tableService;

        public EditTableModel(ITableService tableService)
        {
            _tableService = tableService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public int Number { get; set; }

        [BindProperty]
        public int Seats { get; set; }

        [BindProperty]
        public string Status { get; set; } = string.Empty;

        public IActionResult OnGet(int id)
        {
            var existing = _tableService.GetTableById(id);
            if (existing == null)
                return RedirectToPage("/Index");

            Id = existing.Id;
            Number = existing.Number;
            Seats = existing.Seats;
            Status = existing.Status;
            
            return Page();
        }

        public IActionResult OnPost()
        {
            var updateTableDto = new UpdateTableDTO
            {
                Id = Id,
                Number = Number,
                Seats = Seats,
                Status = Status
            };

            var result = _tableService.UpdateTable(updateTableDto);
            
            if (!result.IsValid)
            {
                ModelState.AddModelError(result.FieldName ?? "", result.ErrorMessage ?? "");
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}
