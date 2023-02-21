using Contracts;
using Contracts.EF;
using Entities.ModelsEf;
using Service.Contracts;
using Shared;

namespace Service.EF;

public sealed class BookEfService : IBookService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _logger;

    public BookEfService(IRepositoryManager repositoryManager, ILoggerManager logger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
    }

    public async Task<BookDto> CreateBook(BookDto bookDto)
    {
        var bookEf = bookDto.ToBookEf();

        if (bookEf == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        _logger.LogInfo($"Create book request received.");

        _repositoryManager.Book.CreateBook(bookEf);

        await _repositoryManager.SaveAsync();

        _logger.LogInfo($"Create book request successfully handled.");

        return bookEf.ToBookDto();
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        var bookEf = await _repositoryManager.Book.GetBook(bookId, false);
        if (bookEf == null)
            throw new Exception($"Couldn't find book with ID: {bookId}");

        _repositoryManager.Book.DeleteBook(bookEf);

        await _repositoryManager.SaveAsync();

        return true;
    }

    public async Task<bool> UpdateBook(BookDto bookDto, string bookId)
    {
        var bookEf = await _repositoryManager.Book.GetBook(bookId, true);
        if (bookEf == null)
            throw new Exception($"Couldn't find book with ID: {bookId}");

        bookEf.UpdateBookEf(bookDto.ToBookEf());

        await _repositoryManager.SaveAsync();

        return true;
    }

    public async Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException(nameof(readBooksRequest));

        _logger.LogInfo($"Read book request received.");

        try
        {
            var booksEf = await _repositoryManager.Book.GetBooks(readBooksRequest, false);

            _logger.LogInfo($"Read book request successfully handled. Returning {booksEf.Count()} book(s).");

            return booksEf.ToBooksDto();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when calling repository from service layer {nameof(ReadBooks)}: {ex.Message}");
            throw;
        }
    }
}
