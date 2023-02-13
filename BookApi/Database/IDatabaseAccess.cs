using BookApi.Data;

namespace BookApi.Database;

public interface IDatabaseAccess
{
    public Book CreateBook(Book book);
    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest);
    public Book UpdateBook(Book book, string bookId);
    public bool DeleteBook(string bookId);

    public string GetValue(GetValueRequest getValueRequest);
}
