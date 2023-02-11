using System.Text.Json;

namespace BookApiServiceTests.TestData;

internal static class TestDataHelper
{
    private static readonly string DataFilePath = Path.Combine(Environment.CurrentDirectory, @"TestData\testbooks.json");

    /// <summary>
    /// Returns a list of TestBook based on IDs provided in the string.
    /// </summary>
    /// <param name="bookIdsToGet">String with book ids separated by spaces. For example "B1 B10 B11 B12"</param>
    public static List<TestBook> GetBooks(string bookIdsToGet = "")
    {
        if (string.IsNullOrWhiteSpace(bookIdsToGet))
            return GetBooksInternal(new List<string>());

        return GetBooksInternal(bookIdsToGet.Split());
    }

    private static List<TestBook> GetBooksInternal(IEnumerable<string> booksToGet)
    {
        var books = GetAllBooks();

        if (!booksToGet.Any())
            return books.ToList();

        return GetSortedSelectionOfBooks(books, booksToGet);
    }

    private static IEnumerable<TestBook> GetAllBooks()
    {
        if (!File.Exists(DataFilePath))
            throw new FileNotFoundException($"Failed to find file: {DataFilePath}");

        var testBooks = JsonSerializer.Deserialize<IEnumerable<TestBook>>(File.ReadAllText(DataFilePath));

        if (!testBooks.Any())
            throw new TestDataLoadException($"Failed to load JSON content from file: {DataFilePath}");

        return testBooks;
    }

    private static List<TestBook> GetSortedSelectionOfBooks(IEnumerable<TestBook> books, IEnumerable<string> booksToGet)
    {
        var selectedBooks = new List<TestBook>();

        foreach (var bookId in booksToGet)
            selectedBooks.Add(books.Single(b => b.Id == bookId));

        return selectedBooks;
    }
}
