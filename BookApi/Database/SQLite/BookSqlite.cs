namespace BookApi.Database.SQLite;

//Used to get full control over the conversion to and from database
//Needed as the naming of PublishDate in Book-object doesn't translate to SQLite publish_date with nameof
public class BookSqlite
{
    public string Id { get; set; }

    public string Author { get; set; }

    public string Title { get; set; }

    public string Genre { get; set; }

    public double Price { get; set; } = double.MinValue;

    public string Publish_date { get; set; }
    
    public string Description { get; set; }

    public static IEnumerable<string> GetPropertyNames()
        => new[]
        {
            nameof(Id),
            nameof(Author),
            nameof(Title),
            nameof(Genre),
            nameof(Price),
            nameof(Publish_date),
            nameof(Description)
        };
}
