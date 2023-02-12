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

    public IEnumerable<Book> GetBooks(ReadBooksRequest readBooksRequest)
    {
        return allBooks;
    }
}
