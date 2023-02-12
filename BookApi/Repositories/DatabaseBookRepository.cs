using BookApi.Data;
using BookApi.Database;

namespace BookApi.Repositories;

public class DatabaseBookRepository : IBookRepository
{
    private readonly IDatabaseAccess _databaseAccess;

    public DatabaseBookRepository(IDatabaseAccess databaseAccess)
    {
        _databaseAccess = databaseAccess;
    }

    public IEnumerable<Book> GetBooks(ReadBooksRequest readBooksRequest)
    {
        var books = _databaseAccess.ReadBooks(readBooksRequest);

        return books;
    }
}
