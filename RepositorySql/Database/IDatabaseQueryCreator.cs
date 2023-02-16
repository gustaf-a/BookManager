using Entities.Data;
using RepositorySql.Database.SQLite;

namespace RepositorySql.Database;

public interface IDatabaseQueryCreator
{
    public SqlQuery Create(Book book);
    public SqlQuery Read(ReadBooksRequest readBooksRequest);
    public SqlQuery Update(Book book, string bookId);
    public SqlQuery Delete(string bookId);
    public SqlQuery GetValueQuery(GetValueRequest getValueRequest);
}
