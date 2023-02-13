using BookApi.Data;

namespace BookApi.Services;

public interface IBookService
{
    public Book CreateBook(Book book);
    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest);
    public Book UpdateBook(Book book, string bookId);
    public bool DeleteBook(string bookId);
}
