using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Student
{
    public class DetailsModel : PageModel
    {
        private readonly StudentService _studentService;

        public DetailsModel(StudentService studentService)
        {
            _studentService = studentService;
        }

        public StudentModel Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            Student = student;
            return Page();
        }
    }
}