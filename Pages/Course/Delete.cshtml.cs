using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Course
{
    public class DeleteModel : PageModel
    {
        private readonly CourseService _courseService;

        public DeleteModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public CourseModel Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            Course = course;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _courseService.DeleteCourseAsync(Course.CourseId);
            if (!result)
                return BadRequest();

            return RedirectToPage("Index");
        }
    }
}