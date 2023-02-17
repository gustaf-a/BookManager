using Contracts;
using RepositorySql.Database;
using Shared;

namespace RepositorySql;

public class DatabaseBookRepository : IBookRepository
{
    private readonly IDatabaseAccess _databaseAccess;
    private readonly IDatabaseIdGenerator _databaseIdGenerator;

    public DatabaseBookRepository(IDatabaseAccess databaseAccess, IDatabaseIdGenerator databaseIdGenerator)
    {
        _databaseAccess = databaseAccess;
        _databaseIdGenerator = databaseIdGenerator;
    }

    public async Task<Book> CreateBook(Book book)
    {
        book.Id = await _databaseIdGenerator.GenerateId();

        var createdBook = await _databaseAccess.CreateBook(book);

        return createdBook;
    }

    public async Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        var books = await _databaseAccess.ReadBooks(readBooksRequest);

        return books;
    }

    public async Task<Book> UpdateBook(Book book)
    {
        var updatedBook = await _databaseAccess.UpdateBook(book);

        return updatedBook;
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        return await _databaseAccess.DeleteBook(bookId);
    }
}
