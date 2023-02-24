using AutoMapper;
using Contracts;
using Service.Contracts;
using Shared;
using Shared.DataTransferObjects;

namespace Service.SQL;

public class BookService : IBookService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IBookRepository _bookRepository;

    private readonly IMapper _mapper;

    public BookService(ILoggerManager loggerManager, IBookRepository bookRepository, IMapper mapper)
    {
        _loggerManager = loggerManager;
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<BookDto> CreateBook(BookForCreationDto bookDto)
    {
        _loggerManager.LogInfo($"Create book request received.");

        var book = _mapper.Map<Book>(bookDto);
        if (book == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        var createdBook = await _bookRepository.CreateBook(book);

        _loggerManager.LogInfo($"Create book request successfully handled. Created: {createdBook.Id}.");

        var bookToReturn = _mapper.Map<BookDto>(createdBook);

        return bookToReturn;
    }

    public async Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        _loggerManager.LogInfo($"Read book request received.");

        var books = await _bookRepository.ReadBooks(readBooksRequest);

        _loggerManager.LogInfo($"Read book request successfully handled. Returning {books.Count()} book(s).");

        var booksToReturn = _mapper.Map<IEnumerable<BookDto>>(books);

        return booksToReturn;
    }

    public async Task<bool> UpdateBook(BookForUpdateDto bookDto, string bookId)
    {
        _loggerManager.LogInfo($"Update book request received for book: {bookId}.");

        var book = _mapper.Map<Book>(bookDto);
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
