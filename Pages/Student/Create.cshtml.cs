using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Student
{
    public class CreateModel : PageModel
    {
        private readonly StudentService _studentService;

        public CreateModel(StudentService studentService)
        {
            _studentService = studentService;
        }

        [BindProperty]
        public StudentModel Student { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _studentService.AddStudentAsync(Student);
            return RedirectToPage("Index");
        }
    }
}