using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Course
{
    public class DetailsModel : PageModel
    {
        private readonly CourseService _courseService;

        public DetailsModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public CourseModel Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            Course = course;
            return Page();
        }
    }
}