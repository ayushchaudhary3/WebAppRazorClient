using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Course
{
    public class IndexModel : PageModel
    {
        private readonly CourseService _courseService;

        public IndexModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public List<CourseModel> Courses { get; set; } = new();

        public async Task OnGetAsync()
        {
            Courses = await _courseService.GetCoursesAsync();
        }
    }
}