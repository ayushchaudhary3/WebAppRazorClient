using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazorClient.Pages
{
    public class SwListModel : PageModel
    {
        private SandwichService? _service;

        public SwListModel(SandwichService service)
        {
            _service = service;
        }

        public Task<List<SandwichModel>> SwList { get; set; } = default!;

        public void OnGet()
        {
            SwList = _service.GetSandwiches();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteSandwichAsync(id);
            return RedirectToPage();
        }

    }
}