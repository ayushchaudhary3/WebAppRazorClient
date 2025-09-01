using System.Text.Json.Serialization;

namespace WebAppRazorClient.Model
{
    public record class EnrollmentModel(
        [property: JsonPropertyName("enrollmentId")] int EnrollmentId,
        [property: JsonPropertyName("studentId")] int StudentId,
        [property: JsonPropertyName("courseId")] int CourseId,
        [property: JsonPropertyName("studentName")] string? StudentName,
        [property: JsonPropertyName("courseTitle")] string? CourseTitle,
        [property: JsonPropertyName("courseCredits")] int? CourseCredits
    );
}