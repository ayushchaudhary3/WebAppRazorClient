using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Enrollment
{
    public class DetailsModel : PageModel
    {
        private readonly EnrollmentService _service;


        public DetailsModel(EnrollmentService service)
        {
            _service = service;
        }
        public EnrollmentModel? Enrollment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Enrollment = await _service.GetEnrollmentByIdAsync(id);
            if (Enrollment == null)
            {
                return NotFound(); // Or redirect to Index with a message
            }
            Enrollment = Enrollment;
            return Page();
        }
    }
}