using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using WebAppRazorClient.Model;

namespace WebAppRazorClient.Service
{
    public class CourseService
    {
        private readonly string _baseUrl = "https://localhost:7113/api/Course";

        public async Task<List<CourseModel>> GetCoursesAsync()
        {
            using HttpClient client = new();
            var stream = await client.GetStreamAsync(_baseUrl);
            var courses = await JsonSerializer.DeserializeAsync<List<CourseModel>>(stream);
            return courses ?? new List<CourseModel>();
        }

        public async Task<CourseModel?> GetCourseByIdAsync(int id)
        {
            using HttpClient client = new();
            var response = await client.GetAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseModel>();
        }

        public async Task<CourseModel?> AddCourseAsync(CourseModel course)
        {
            using HttpClient client = new();
            var response = await client.PostAsJsonAsync(_baseUrl, course);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseModel>();
        }

        public async Task<bool> UpdateCourseAsync(int id, CourseModel course)
        {
            using HttpClient client = new();
            var response = await client.PutAsJsonAsync($"{_baseUrl}/{id}", course);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            using HttpClient client = new();
            var response = await client.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}