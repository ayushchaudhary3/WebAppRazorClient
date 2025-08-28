using System.Text.Json.Serialization;

namespace WebAppRazorClient
{
    public record class SandwichModel(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("price")] double Price
        );
}