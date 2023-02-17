using Contracts;
using Service.Contracts;
using Shared;

namespace BookApi.Services;

public class BookService : IBookService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IBookRepository _bookRepository;

    public BookService(ILoggerManager loggerManager, IBookRepository bookRepository)
    {
        _loggerManager = loggerManager;
        _bookRepository = bookRepository;
    }

    public async Task<Book> CreateBook(Book book)
    {
        _loggerManager.LogInfo($"Create book request received.");

        var createdBook = await _bookRepository.CreateBook(book);

        _loggerManager.LogInfo($"Create book request successfully handled. Created: {createdBook.Id}.");

        return createdBook;
    }

    public async Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        _loggerManager.LogInfo($"Read book request received.");

        var books = await _bookRepository.ReadBooks(readBooksRequest);

        _loggerManager.LogInfo($"Read book request successfully handled. Returning {books.Count()} book(s).");

        return books;
    }

    public async Task<Book> UpdateBook(Book book, string bookId)
    {
        _loggerManager.LogInfo($"Update book request received for book: {bookId}.");

        book.Id = bookId;

        var updatedBook = await _bookRepository.UpdateBook(book);

        _loggerManager.LogInfo($"Update book request successfully handled. Book {updatedBook.Id} updated.");

        return updatedBook;
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        _loggerManager.LogInfo($"Delete book request received for book: {bookId}.");

        var deleteResponse = await _bookRepository.DeleteBook(bookId);

        var logMessage = deleteResponse
            ? $"Delete book request successfully handled. Book {bookId} deleted."
            : $"Delete book request failed for book: {bookId}";

        _loggerManager.LogInfo(logMessage);

        return deleteResponse;
    }

}
