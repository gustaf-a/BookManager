using Entities.ModelsSql;
using Shared;

namespace RepositorySql;

public interface IDatabaseQueryCreator
{
    public SqlQuery Create(BookSqlite book);
    public SqlQuery Read(ReadBooksRequest readBooksRequest);
    public SqlQuery Update(BookSqlite book);
    public SqlQuery Delete(string bookId);
    public SqlQuery GetValueQuery(GetValueRequest getValueRequest);
}
