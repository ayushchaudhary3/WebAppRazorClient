using System.Text.Json.Serialization;

namespace WebAppRazorClient.Model
{
    public record class StudentModel(
        [property: JsonPropertyName("studentId")] int StudentId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("age")] int Age,
        [property: JsonPropertyName("address")] string Address
    );
}