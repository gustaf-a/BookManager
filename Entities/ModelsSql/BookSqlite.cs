using RepositorySql.Database.SQLite;

namespace Entities.ModelsSql;

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

    public Dictionary<string, object> GetProperties(bool includeDefaultValues = true, bool includeId = false)
    {
        var properties = new Dictionary<string, object>();

        if (includeId)
            properties.AddIfNotDefault(Id, nameof(Id));

        properties.AddIfNotDefault(Author, nameof(Author));
        properties.AddIfNotDefault(Title, nameof(Title));
        properties.AddIfNotDefault(Genre, nameof(Genre));
        properties.AddIfNotDefault(Price, nameof(Price));
        properties.AddIfNotDefault(Publish_date, nameof(Publish_date));
        properties.AddIfNotDefault(Description, nameof(Description));

        return properties;
    }
}
