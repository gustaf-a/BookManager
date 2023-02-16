using Entities.Data;

namespace Contracts;

public interface IBookRepository
{
    public Task<Book> CreateBook(Book book);
    public Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<Book> UpdateBook(Book book, string bookId);
    public Task<bool> DeleteBook(string bookId);

}
