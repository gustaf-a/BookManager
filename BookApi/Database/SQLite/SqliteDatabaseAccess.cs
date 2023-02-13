using BookApi.Configuration;
using BookApi.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BookApi.Database.SQLite;

/// <summary>
/// SQLite database access class using Dapper with parameters for protection against SQL injection
/// </summary>
public class SqliteDatabaseAccess : IDatabaseAccess
{
    private readonly string _connectionString;

    private readonly IDatabaseQueryCreator _queryCreator;

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

    // --------------------- COMMON ------------------------------------

    private Book GetBookById(Book book)
    {
        return ReadBooks(new ReadBooksRequest
        {
            FilterByTextValue = book.Id
        }).First();
    }

    private int ExecuteQuery(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        var affectedRows = connection.Execute(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));

        return affectedRows;
    }

    // --------------------- CREATE ------------------------------------

    public Book CreateBook(Book book)
    {
        var sqlQuery = _queryCreator.Create(book);

        var affectedRows = ExecuteQuery(sqlQuery);

        if (affectedRows == 0)
            throw new Exception("Database failed to create new book.");

        return GetBookById(book);
    }

    // --------------------- READ ------------------------------------

    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        var query = _queryCreator.Read(readBooksRequest);

        var result = ExecuteReaderQuery(query);

        return result;
    }

    private IEnumerable<Book> ExecuteReaderQuery(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = connection.Query<BookSqlite>(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));

        return result.ToBooks();
    }

    // --------------------- UPDATE ------------------------------------

    public Book UpdateBook(Book book, string bookId)
    {
        var sqlQuery = _queryCreator.Update(book, bookId);

        var affectedRows = ExecuteQuery(sqlQuery);

        if (affectedRows == 0)
            throw new Exception($"Database failed to update book with ID {bookId}.");

        return GetBookById(book);
    }

    // --------------------- DELETE ------------------------------------

    public bool DeleteBook(string bookId)
    {
        var sqlQuery = _queryCreator.Delete(bookId);

        var affectedRows = ExecuteQuery(sqlQuery);

        //If book doesn't exist, consider delete success.
        if (affectedRows == 0 && CheckIfBookExists(bookId))
            throw new Exception($"Database failed to delete book with ID {bookId}");

        return true;
    }

    private bool CheckIfBookExists(string bookId)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FilterByTextValue = bookId
        };

        var result = ReadBooks(readBooksRequest);

        return result.Any();
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
