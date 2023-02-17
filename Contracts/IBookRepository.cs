using Shared;

namespace Contracts;

public interface IBookRepository
{
    public Task<Book> CreateBook(Book book);
    public Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<Book> UpdateBook(Book book);
    public Task<bool> DeleteBook(string bookId);

}
