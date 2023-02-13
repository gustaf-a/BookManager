using BookApi.Data;
using System.Text.Json;

namespace BookApi.Repositories;

public class JsonFileBookRepository : IBookRepository
{
    private static readonly string DataFilePath = Path.Combine(Environment.CurrentDirectory, @"books.json");

    private readonly List<Book> allBooks = new();

    public JsonFileBookRepository()
    {
        allBooks = JsonSerializer.Deserialize<IEnumerable<Book>>(File.ReadAllText(DataFilePath)).ToList();
    }

    public Book CreateBook(Book book)
    {
        throw new NotImplementedException();
    }

    public bool DeleteBook(string bookId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        return allBooks;
    }

    public Book UpdateBook(Book book, string bookId)
    {
        throw new NotImplementedException();
    }
}
