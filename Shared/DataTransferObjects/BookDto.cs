using Entities.JsonConverters;
using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects;

/// <summary>
/// Data Transfer Object used to send Book data.
/// </summary>
public record BookDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    [JsonConverter(typeof(DoubleWithDotJsonConverter))]
    public double Price { get; set; } = double.MinValue;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("genre")]
    public string Genre { get; set; } = string.Empty;

    [JsonPropertyName("publish_date")]
    public string Publish_date { get; set; } = string.Empty;
}
