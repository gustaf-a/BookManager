using BookApi.Data;
using BookApi.Database;

namespace BookApi.Repositories;

public class DatabaseBookRepository : IBookRepository
{
    private readonly IDatabaseAccess _databaseAccess;
    private readonly IDatabaseIdGenerator _databaseIdGenerator;

    public DatabaseBookRepository(IDatabaseAccess databaseAccess, IDatabaseIdGenerator databaseIdGenerator)
    {
        _databaseAccess = databaseAccess;
        _databaseIdGenerator = databaseIdGenerator;
    }

    public Book CreateBook(Book book)
    {
        book.Id = _databaseIdGenerator.GenerateId();

        var createdBook = _databaseAccess.CreateBook(book);

        return createdBook;
    }

    public bool DeleteBook(string bookId)
    {
        return _databaseAccess.DeleteBook(bookId);
    }

    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        var books = _databaseAccess.ReadBooks(readBooksRequest);

        return books;
    }

    public Book UpdateBook(Book book, string bookId)
    {
        var updatedBook = _databaseAccess.UpdateBook(book, bookId);

        return updatedBook;
    }
}
