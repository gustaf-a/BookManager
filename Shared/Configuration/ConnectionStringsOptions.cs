namespace Shared.Configuration;

public class ConnectionStringsOptions
{
    public const string ConnectionString = "ConnectionStrings";

    public string sqlite { get; set; } = "";
    public string sqlConnection { get; set; } = "";
}
