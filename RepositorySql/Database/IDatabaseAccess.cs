using Entities.Data;

namespace RepositorySql.Database;

public interface IDatabaseAccess
{
    public Task<Book> CreateBook(Book book);
    public Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<Book> UpdateBook(Book book);
    public Task<bool> DeleteBook(string bookId);

    public Task<string> GetValue(GetValueRequest getValueRequest);
}
