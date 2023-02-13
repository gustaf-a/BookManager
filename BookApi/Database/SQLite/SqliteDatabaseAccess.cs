using BookApi.Configuration;
using BookApi.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BookApi.Database.SQLite;

public class SqliteDatabaseAccess : IDatabaseAccess
{
    private readonly string _connectionString;

    private IDatabaseQueryCreator _queryCreator;

    public SqliteDatabaseAccess(IConfiguration configuration, IDatabaseQueryCreator queryCreator)
    {
        var databaseOptions = configuration.GetSection(DatabaseOptions.Database).Get<DatabaseOptions>();

        if (string.IsNullOrWhiteSpace(databaseOptions?.SqliteConnectionStringName))
            throw new Exception($"Failed to find ConnectionString name for {nameof(databaseOptions.SqliteConnectionStringName)}");

        _connectionString = configuration.GetConnectionString(databaseOptions.SqliteConnectionStringName);

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new Exception($"Invalid ConnectionString: {databaseOptions.SqliteConnectionStringName}");

        _queryCreator = queryCreator;
    }

    public Book CreateBook(Book book)
    {
        throw new NotImplementedException();
    }

    public bool DeleteBook(Book book)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        var query = _queryCreator.Read(readBooksRequest);

        var result = ExecuteReaderQuery(query);

        return result;
    }

    public Book UpdateBook(Book book)
    {
        throw new NotImplementedException();
    }

    //using Dapper for queries helps preventing SQL Injection
    private IEnumerable<Book> ExecuteReaderQuery(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = connection.Query<BookSqlite>(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));

        return result.ToBooks();
    }
    // --------------------- GET VALUE ------------------------------------

    public string GetValue(GetValueRequest getValueRequest)
    {
        var sqlQuery = _queryCreator.GetValueQuery(getValueRequest);

        return ExecuteScalarQuery<string>(sqlQuery);
    }

    private T ExecuteScalarQuery<T>(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        return connection.ExecuteScalar<T>(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));
    }
}
