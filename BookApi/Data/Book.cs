using System.Text.Json.Serialization;

namespace BookApi.Data;

public class Book
{
    public string Id { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    
    [JsonPropertyName("published_date")]
    public DateTime PublishedDate { get; set; }
}
