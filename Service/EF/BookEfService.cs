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
        //Conforms to RepositorySql pattern and validates date.
        //To be replaced with better validation to allow direct BookDto->BookEf conversion. (2023-02-21)
        var book = bookDto.ToBook();

        if (book == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        _logger.LogInfo($"Create book request received.");

        _repositoryManager.Book.CreateBook(book.ToBookEf());

        await _repositoryManager.SaveAsync();

        _logger.LogInfo($"Create book request successfully handled.");

        return book.ToBookDto();
    }

    public Task<bool> DeleteBook(string bookId)
    {
        throw new NotImplementedException();
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

    public Task<bool> UpdateBook(BookDto BookDto, string bookId)
    {
        throw new NotImplementedException();
    }
}
