using Contracts;
using Service.Contracts;
using Shared;

namespace Service.SQL;

public class BookService : IBookService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IBookRepository _bookRepository;

    public BookService(ILoggerManager loggerManager, IBookRepository bookRepository)
    {
        _loggerManager = loggerManager;
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> CreateBook(BookDto bookDto)
    {
        _loggerManager.LogInfo($"Create book request received.");

        var book = bookDto.ToBook();

        if (book == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        var createdBook = await _bookRepository.CreateBook(book);

        _loggerManager.LogInfo($"Create book request successfully handled. Created: {createdBook.Id}.");

        return createdBook.ToBookDto();
    }

    public async Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        _loggerManager.LogInfo($"Read book request received.");

        var books = await _bookRepository.ReadBooks(readBooksRequest);

        _loggerManager.LogInfo($"Read book request successfully handled. Returning {books.Count()} book(s).");

        return books.ToBooksDto();
    }

    public async Task<bool> UpdateBook(BookDto bookDto, string bookId)
    {
        _loggerManager.LogInfo($"Update book request received for book: {bookId}.");

        var book = bookDto.ToBook();

        if (book == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        book.Id = bookId;

        var updatedBook = await _bookRepository.UpdateBook(book);

        _loggerManager.LogInfo($"Update book request successfully handled. Book {updatedBook.Id} updated.");

        return true;
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
