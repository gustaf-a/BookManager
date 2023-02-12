using BookApi.Data;
using BookApi.Database.SQLite;

namespace BookApi.Database;

public interface IDatabaseQueryCreator
{
    public string Create(Book book);
    public SqlQuery Read(ReadBooksRequest readBooksRequest);
    public string Update(Book book);
    public string Delete(Book book);
}
