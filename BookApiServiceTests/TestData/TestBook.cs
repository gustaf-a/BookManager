using System.Text.Json.Serialization;

namespace BookApiServiceTests.TestData;

internal class TestBook
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("price")]
    [JsonConverter(typeof(DoubleWithDotJsonConverter))]
    public double Price { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("genre")]
    public string Genre { get; set; }

    //Attribute using custom JsonConverter needed to convert yyyy-MM-dd 
    [JsonConverter(typeof(DateOnlyParseExactJsonConverter))] //JsonSerializer
    //[JsonConverter(typeof(DateOnlyNewtonSoftJsonConverter))] //NewtonSoft
    [JsonPropertyName("publish_date")]
    public DateOnly PublishDate { get; set; }
}
