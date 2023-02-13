using BookApi.Data;

namespace BookApi.Database;

public interface IDatabaseAccess
{
    public Book CreateBook(Book book);
    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest);
    public Book UpdateBook(Book book);
    public bool DeleteBook(Book book);

    public string GetValue(GetValueRequest getValueRequest);
}
