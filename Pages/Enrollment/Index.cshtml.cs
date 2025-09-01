using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Enrollment
{
    public class IndexModel : PageModel
    {
        private readonly EnrollmentService _enrollmentService;

        public IndexModel(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public List<EnrollmentModel> Enrollments { get; set; } = new();

        public async Task OnGetAsync()
        {
            Enrollments = await _enrollmentService.GetEnrollmentsAsync();
        }
    }
}