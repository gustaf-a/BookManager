using RepositorySql.Database.SQLite;
using Shared;

namespace RepositorySql.Database;

public interface IDatabaseQueryCreator
{
    public SqlQuery Create(Book book);
    public SqlQuery Read(ReadBooksRequest readBooksRequest);
    public SqlQuery Update(Book book);
    public SqlQuery Delete(string bookId);
    public SqlQuery GetValueQuery(GetValueRequest getValueRequest);
}
