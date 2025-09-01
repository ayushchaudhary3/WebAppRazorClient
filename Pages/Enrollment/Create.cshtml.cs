using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Enrollment
{
    public class CreateModel : PageModel
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly StudentService _studentService;
        private readonly CourseService _courseService;

        public CreateModel(
            EnrollmentService enrollmentService,
            StudentService studentService,
            CourseService courseService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _courseService = courseService;
        }

        [BindProperty]
        public EnrollmentModel Enrollment { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var students = await _studentService.GetStudentsAsync();
            var courses = await _courseService.GetCoursesAsync();

            ViewData["StudentId"] = new SelectList(
                students, "StudentId", "Name"
            );
            ViewData["CourseId"] = new SelectList(
                courses, "CourseId", "Title"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var students = await _studentService.GetStudentsAsync();
                var courses = await _courseService.GetCoursesAsync();
                ViewData["StudentId"] = new SelectList(students, "StudentId", "Name");
                ViewData["CourseId"] = new SelectList(courses, "CourseId", "Title");
                return Page();
            }

            await _enrollmentService.AddEnrollmentAsync(Enrollment);
            return RedirectToPage("Index");
        }
    }
}