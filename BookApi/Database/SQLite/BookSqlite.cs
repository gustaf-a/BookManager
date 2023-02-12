namespace BookApi.Database.SQLite;

//Used to get full control over the conversion to and from database
public class BookSqlite
{
    public string Id { get; set; }

    public string Author { get; set; }

    public string Title { get; set; }

    public double Price { get; set; }

    public string Description { get; set; }

    public string Genre { get; set; }

    public string Publish_date { get; set; }
}
