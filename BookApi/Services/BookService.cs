using Contracts;
using Entities.Data;
using Serilog;

namespace BookApi.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Book> CreateBook(Book book)
    {
        Log.Information($"Create book request received.");

        var createdBook = await _bookRepository.CreateBook(book);

        Log.Information($"Create book request successfully handled. Created: {createdBook.Id}.");

        return createdBook;
    }

    public async Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        Log.Information($"Read book request received.");

        var books = await _bookRepository.ReadBooks(readBooksRequest);

        Log.Information($"Read book request successfully handled. Returning {books.Count()} book(s).");

        return books;
    }

    public async Task<Book> UpdateBook(Book book, string bookId)
    {
        Log.Information($"Update book request received for book: {bookId}.");

        var updatedBook = await _bookRepository.UpdateBook(book, bookId);

        Log.Information($"Update book request successfully handled. Book {updatedBook} updated.");

        return updatedBook;
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        Log.Information($"Delete book request received for book: {bookId}.");

        var deleteResponse = await _bookRepository.DeleteBook(bookId);

        var logMessage = deleteResponse
            ? $"Delete book request successfully handled. Book {bookId} deleted."
            : $"Delete book request failed for book: {bookId}";

        Log.Information(logMessage);

        return deleteResponse;
    }

}
