using System.Text.Json.Serialization;

namespace WebAppRazorClient.Model
{
    public record class CourseModel(
        [property: JsonPropertyName("courseId")] int CourseId,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("credits")] int Credits
    );
}