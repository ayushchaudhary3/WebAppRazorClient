using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Enrollment
{
    public class DeleteModel : PageModel
    {
        private readonly EnrollmentService _enrollmentService;

        public DeleteModel(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [BindProperty]
        public EnrollmentModel Enrollment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
                return NotFound();

            Enrollment = enrollment;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _enrollmentService.DeleteEnrollmentAsync(Enrollment.EnrollmentId);
            if (!result)
                return BadRequest();

            return RedirectToPage("Index");
        }
    }
}