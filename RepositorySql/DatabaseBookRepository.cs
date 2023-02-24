using AutoMapper;
using Contracts;
using Entities.ModelsSql;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Configuration;

namespace RepositorySql;

public class DatabaseBookRepository : IBookRepository
{
    private readonly DatabaseOptions _databaseOptions;

    private readonly IDatabaseAccess _databaseAccess;
    private readonly IIdGenerator _databaseIdGenerator;

    private readonly IMapper _mapper;

    public DatabaseBookRepository(IOptions<DatabaseOptions> databaseOptions, IDatabaseAccess databaseAccess, IIdGenerator databaseIdGenerator, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(databaseOptions?.Value);
        _databaseOptions = databaseOptions.Value;

        _databaseAccess = databaseAccess;
        _databaseIdGenerator = databaseIdGenerator;

        _mapper = mapper;
    }

    public async Task<Book> CreateBook(Book book)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        book.Id = await GetBookId();

        var bookSqlite = _mapper.Map<BookSqlite>(book);

        var createdBookSqlite = await _databaseAccess.CreateBook(bookSqlite);

        var createdBook = _mapper.Map<Book>(createdBookSqlite);

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

        var booksSqlite = await _databaseAccess.ReadBooks(readBooksRequest);

        var books = _mapper.Map<IEnumerable<Book>>(booksSqlite);

        return  books;
    }

    public async Task<Book> UpdateBook(Book book)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        var bookSqlite = _mapper.Map<BookSqlite>(book);

        var updatedBookSqlite = await _databaseAccess.UpdateBook(bookSqlite);

        var updatedBook = _mapper.Map<Book>(updatedBookSqlite);

        return updatedBook;
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        if (string.IsNullOrWhiteSpace(bookId))
            throw new ArgumentNullException(nameof(bookId));

        return await _databaseAccess.DeleteBook(bookId);
    }
}
