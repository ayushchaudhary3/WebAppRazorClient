using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazorClient.Pages
{
    public class CreateModel : PageModel
    {
        private readonly SandwichService _service;

        public CreateModel(SandwichService service)
        {
            _service = service;
        }

        [BindProperty]
        public SandwichModel Sandwich { get; set; }

        public List<SandwichModel> SwList { get; set; } = new();

        public async Task OnGetAsync()
        {
            SwList = await _service.GetSandwiches();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                SwList = await _service.GetSandwiches();
                return Page();
            }

            await _service.AddSandwich(Sandwich);

            // Redirect to Sandwich List page after successful creation
            return RedirectToPage("/SwList");
        }
    }
}
