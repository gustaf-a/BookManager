using System.Text;

namespace BookApi.Database.SQLite;

public class SqlQuery
{
    public Dictionary<string, object> Parameters;
    public StringBuilder QueryString;

    public SqlQuery()
    {
        Parameters = new();
        QueryString = new();
    }
}
