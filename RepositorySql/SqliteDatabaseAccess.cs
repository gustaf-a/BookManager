using Dapper;
using Entities.Exceptions;
using Entities.ModelsSql;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Configuration;

namespace RepositorySql;

/// <summary>
/// SQLite database access class using Dapper with parameters for protection against SQL injection
/// </summary>
public class SqliteDatabaseAccess : IDatabaseAccess
{
    private readonly string _connectionString;

    private readonly IDatabaseQueryCreator _queryCreator;

    public SqliteDatabaseAccess(IOptions<DatabaseOptions> optionsDatabase, IOptions<ConnectionStringsOptions> connectionStringsOptions, IDatabaseQueryCreator queryCreator)
    {
        var databaseOptions = optionsDatabase.Value;

        if (string.IsNullOrWhiteSpace(databaseOptions?.SqliteConnectionStringName))
            throw new Exception($"Failed to find ConnectionString name for {nameof(databaseOptions.SqliteConnectionStringName)}");

        _connectionString = connectionStringsOptions.Value.sqlite;

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new Exception($"Invalid ConnectionString: {databaseOptions.SqliteConnectionStringName}");

        _queryCreator = queryCreator;
    }

    // --------------------- COMMON ------------------------------------

    private async Task<Book> GetBook(string bookId)
    {
        var booksResult = await ReadBooks(new ReadBooksRequest
        {
            FilterByTextValue = bookId
        });

        if (booksResult.Any())
            throw new BookNotFoundException(bookId);

        if (booksResult.Count() > 1)
            throw new TooManyBooksFoundException(bookId, booksResult.Count());

        return booksResult.First();
    }

    private async Task<int> ExecuteQueryAsync(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        var affectedRows = await connection.ExecuteAsync(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));

        return affectedRows;
    }

    // --------------------- CREATE ------------------------------------

    public async Task<Book> CreateBook(Book book)
    {
        var sqlQuery = _queryCreator.Create(book);

        var affectedRows = await ExecuteQueryAsync(sqlQuery);

        if (affectedRows == 0)
            throw new Exception("Database failed to create new book.");

        return await GetBook(book.Id);
    }

    // --------------------- READ ------------------------------------

    public async Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        var query = _queryCreator.Read(readBooksRequest);

        var result = await ExecuteReaderQuery(query);

        return result;
    }

    private async Task<IEnumerable<Book>> ExecuteReaderQuery(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryAsync<BookSqlite>(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));

        return result.ToBooks();
    }

    // --------------------- UPDATE ------------------------------------

    public async Task<Book> UpdateBook(Book book)
    {
        var sqlQuery = _queryCreator.Update(book);

        var affectedRows = await ExecuteQueryAsync(sqlQuery);

        if (affectedRows == 0)
            throw new Exception($"Database failed to update book with ID {book.Id}.");

        return await GetBook(book.Id);
    }

    // --------------------- DELETE ------------------------------------

    public async Task<bool> DeleteBook(string bookId)
    {
        var sqlQuery = _queryCreator.Delete(bookId);

        var affectedRows = await ExecuteQueryAsync(sqlQuery);

        //If book doesn't exist, consider delete success.
        if (affectedRows == 0 && await CheckIfBookExists(bookId))
            throw new Exception($"Database failed to delete book with ID {bookId}");

        return true;
    }

    private async Task<bool> CheckIfBookExists(string bookId)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FilterByTextValue = bookId
        };

        var result = await ReadBooks(readBooksRequest);

        return result.Any();
    }

    // --------------------- GET VALUE ------------------------------------

    public async Task<string> GetValue(GetValueRequest getValueRequest)
    {
        var sqlQuery = _queryCreator.GetValueQuery(getValueRequest);

        return await ExecuteScalarQuery<string>(sqlQuery);
    }

    private async Task<T> ExecuteScalarQuery<T>(SqlQuery sqlQuery)
    {
        using var connection = new SqliteConnection(_connectionString);

        return await connection.ExecuteScalarAsync<T>(sqlQuery.QueryString.ToString(), new DynamicParameters(sqlQuery.Parameters));
    }
}
