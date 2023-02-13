using BookApi.Data;

namespace BookApi.Repositories;

public interface IBookRepository
{
    public Book CreateBook(Book book);
    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest);
    public Book UpdateBook(Book book, string bookId);
    public bool DeleteBook(string bookId);

}
