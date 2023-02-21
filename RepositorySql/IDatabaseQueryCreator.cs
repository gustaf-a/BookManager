using Shared;

namespace RepositorySql;

public interface IDatabaseQueryCreator
{
    public SqlQuery Create(Book book);
    public SqlQuery Read(ReadBooksRequest readBooksRequest);
    public SqlQuery Update(Book book);
    public SqlQuery Delete(string bookId);
    public SqlQuery GetValueQuery(GetValueRequest getValueRequest);
}
