using BookApi.Repositories.JsonConverters;
using System.Text.Json.Serialization;

namespace BookApi.Data;

public class Book
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("price")]
    [JsonConverter(typeof(DoubleWithDotJsonConverter))]
    public double Price { get; set; } = double.MinValue;

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("genre")]
    public string Genre { get; set; }

    [JsonPropertyName("publish_date")]
    public DateOnly PublishDate { get; set; }
}
