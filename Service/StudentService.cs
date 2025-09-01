using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using WebAppRazorClient.Model;

namespace WebAppRazorClient.Service
{
    public class StudentService
    {
        private readonly string _baseUrl = "https://localhost:7113/api/Student";

        public async Task<List<StudentModel>> GetStudentsAsync()
        {
            using HttpClient client = new();
            var stream = await client.GetStreamAsync(_baseUrl);
            var students = await JsonSerializer.DeserializeAsync<List<StudentModel>>(stream);
            return students ?? new List<StudentModel>();
        }

        public async Task<StudentModel?> GetStudentByIdAsync(int id)
        {
            using HttpClient client = new();
            var response = await client.GetAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StudentModel>();
        }

        public async Task<StudentModel?> AddStudentAsync(StudentModel student)
        {
            using HttpClient client = new();
            var response = await client.PostAsJsonAsync(_baseUrl, student);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StudentModel>();
        }

        public async Task<bool> UpdateStudentAsync(int id, StudentModel student)
        {
            using HttpClient client = new();
            var response = await client.PutAsJsonAsync($"{_baseUrl}/{id}", student);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using HttpClient client = new();
            var response = await client.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}