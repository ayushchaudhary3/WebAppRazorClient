using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Model;
using WebAppRazorClient.Service;

namespace WebAppRazorClient.Pages.Student
{
    public class IndexModel : PageModel
    {
        private readonly StudentService _studentService;

        public IndexModel(StudentService studentService)
        {
            _studentService = studentService;
        }

        public List<StudentModel> Students { get; set; } = new();

        public async Task OnGetAsync()
        {
            Students = await _studentService.GetStudentsAsync();
        }
    }
}