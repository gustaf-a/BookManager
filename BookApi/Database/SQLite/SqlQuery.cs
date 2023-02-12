namespace BookApi.Database.SQLite;

public class SqlQuery
{
    public string QueryString;
    public Dictionary<string, object> Parameters;

    public SqlQuery()
    {
        Parameters = new();
    }
}
