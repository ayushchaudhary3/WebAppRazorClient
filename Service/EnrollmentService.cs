using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using WebAppRazorClient.Model;

namespace WebAppRazorClient.Service
{
    public class EnrollmentService
    {
        private readonly string _baseUrl = "https://localhost:7113/api/Enrollment";

        public async Task<List<EnrollmentModel>> GetEnrollmentsAsync()
        {
            using HttpClient client = new();
            var stream = await client.GetStreamAsync(_baseUrl);
            var enrollments = await JsonSerializer.DeserializeAsync<List<EnrollmentModel>>(stream);
            return enrollments ?? new List<EnrollmentModel>();
        }

        public async Task<EnrollmentModel?> GetEnrollmentByIdAsync(int id)
        {
            using HttpClient client = new();
            var response = await client.GetAsync($"{_baseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<EnrollmentModel>();
        }

        public async Task<EnrollmentModel?> AddEnrollmentAsync(EnrollmentModel enrollment)
        {
            using HttpClient client = new();
            // Only send StudentId and CourseId for creation
            var createDto = new
            {
                studentId = enrollment.StudentId,
                courseId = enrollment.CourseId
            };
            var response = await client.PostAsJsonAsync(_baseUrl, createDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EnrollmentModel>();
        }

        public async Task<bool> UpdateEnrollmentAsync(int id, EnrollmentModel enrollment)
        {
            using HttpClient client = new();
            var response = await client.PutAsJsonAsync($"{_baseUrl}/{id}", enrollment);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            using HttpClient client = new();
            var response = await client.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}