using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Student
{
    public class EditModel : PageModel
    {
        private readonly StudentService _studentService;

        public EditModel(StudentService studentService)
        {
            _studentService = studentService;
        }

        [BindProperty]
        public StudentModel Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            Student = student;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _studentService.UpdateStudentAsync(Student.StudentId, Student);
            if (!result)
                return BadRequest();

            return RedirectToPage("Index");
        }
    }
}