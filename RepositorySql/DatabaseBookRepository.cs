using Contracts;
using Entities.ModelsSql;
using Microsoft.Extensions.Options;
using RepositorySql.Configuration;
using Shared;

namespace RepositorySql;

public class DatabaseBookRepository : IBookRepository
{
    private readonly DatabaseOptions _databaseOptions;

    private readonly IDatabaseAccess _databaseAccess;
    private readonly IDatabaseIdGenerator _databaseIdGenerator;

    public DatabaseBookRepository(IOptions<DatabaseOptions> databaseOptions, IDatabaseAccess databaseAccess, IDatabaseIdGenerator databaseIdGenerator)
    {
        ArgumentNullException.ThrowIfNull(databaseOptions?.Value);
        _databaseOptions = databaseOptions.Value;
        
        _databaseAccess = databaseAccess;
        _databaseIdGenerator = databaseIdGenerator;
    }

    public async Task<Book> CreateBook(Book book)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        book.Id = await GetBookId();

        var createdBook = await _databaseAccess.CreateBook(book);

        return createdBook;
    }

    private async Task<string> GetBookId()
    {
        var getValuesRequest = new GetValueRequest
        {
            ColumnName = nameof(BookSqlite.Id),
            IgnoreFirstCharacters = _databaseOptions.IdCharacterPrefixLength,
            GetMaxValue = true
        };

        var currentMaxId = await _databaseAccess.GetValue(getValuesRequest);

        return _databaseIdGenerator.GenerateId(currentMaxId);
    }

    public async Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest is null)
            throw new ArgumentNullException(nameof(readBooksRequest));

        var books = await _databaseAccess.ReadBooks(readBooksRequest);

        return books;
    }

    public async Task<Book> UpdateBook(Book book)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        var updatedBook = await _databaseAccess.UpdateBook(book);

        return updatedBook;
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        if (string.IsNullOrWhiteSpace(bookId))
            throw new ArgumentNullException(nameof(bookId));

        return await _databaseAccess.DeleteBook(bookId);
    }
}
