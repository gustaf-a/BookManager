using Entities.ModelsSql;
using Shared;

namespace RepositorySql;

public interface IDatabaseAccess
{
    public Task<BookSqlite> CreateBook(BookSqlite book);
    public Task<IEnumerable<BookSqlite>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<BookSqlite> UpdateBook(BookSqlite book);
    public Task<bool> DeleteBook(string bookId);

    public Task<string> GetValue(GetValueRequest getValueRequest);
}
