using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient;

namespace SandwichFront.Pages
{
    public class EditModel : PageModel
    {
        private readonly SandwichService _service;
        public EditModel(SandwichService service)
        {
            _service = service;
        }
        [BindProperty]
        public SandwichModel Sandwich { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Sandwich = await _service.GetSandwichById(id);
            if (Sandwich == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _service.UpdateSandwich(Sandwich.Id, Sandwich);
            return RedirectToPage("SwList");
        }
    }
}