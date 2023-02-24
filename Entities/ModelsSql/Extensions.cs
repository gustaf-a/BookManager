using Shared;

namespace Entities.ModelsSql;

public static class Extensions
{
    public static string ToBookSqliteName(this string bookName)
        => bookName switch
        {
            nameof(Book.Id) => nameof(BookSqlite.Id).ToLower(),
            nameof(Book.Author) => nameof(BookSqlite.Author).ToLower(),
            nameof(Book.Description) => nameof(BookSqlite.Description).ToLower(),
            nameof(Book.Genre) => nameof(BookSqlite.Genre).ToLower(),
            nameof(Book.Price) => nameof(BookSqlite.Price).ToLower(),
            nameof(Book.PublishDate) => nameof(BookSqlite.Publish_date).ToLower(),
            nameof(Book.Title) => nameof(BookSqlite.Title).ToLower(),
            _ => throw new NotImplementedException($"Property name conversion not implemented for {bookName}")
        };

    public static void AddIfNotDefault(this Dictionary<string, object> dictionary, string stringValue, string key)
    {
        if (!string.IsNullOrEmpty(stringValue))
            dictionary.Add(key, stringValue);
    }

    public static void AddIfNotDefault(this Dictionary<string, object> dictionary, double doubleValue, string key)
    {
        if (doubleValue > double.MinValue)
            dictionary.Add(key, doubleValue);
    }
}
