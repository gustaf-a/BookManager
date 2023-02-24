using AutoMapper;
using Contracts;
using Contracts.EF;
using Entities.Exceptions;
using Entities.ModelsEf;
using Service.Contracts;
using Shared;
using Shared.DataTransferObjects;

namespace Service.EF;

public sealed class BookEfService : IBookService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _logger;

    private readonly IMapper _mapper;

    public BookEfService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BookDto> CreateBook(BookForCreationDto bookDto)
    {
        var bookEf = _mapper.Map<BookEf>(bookDto);
        if (bookEf == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        _logger.LogInfo($"Create book request received.");

        _repositoryManager.Book.CreateBook(bookEf);

        await _repositoryManager.SaveAsync();

        _logger.LogInfo($"Create book request successfully handled. Created: {bookEf.Id}.");

        var bookToReturn = _mapper.Map<BookDto>(bookEf);

        return bookToReturn;
    }

    public async Task<bool> UpdateBook(BookForUpdateDto bookDto, string bookId)
    {
        var bookEfFromInput = _mapper.Map<BookEf>(bookDto);
        if (bookEfFromInput == null)
            throw new Exception("Failed to parse input data. Please review data sent.");

        var bookEf = await _repositoryManager.Book.GetBook(bookId, true);
        if (bookEf == null)
            throw new BookNotFoundException(bookId);

        bookEf.UpdateBookEf(bookEfFromInput);

        await _repositoryManager.SaveAsync();

        return true;
    }

    public async Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException(nameof(readBooksRequest));

        _logger.LogInfo($"Read book request received.");

        var booksEf = await _repositoryManager.Book.GetBooks(readBooksRequest, false);

        _logger.LogInfo($"Read book request successfully handled. Returning {booksEf.Count()} book(s).");

        var booksToReturn = _mapper.Map<IEnumerable<BookDto>>(booksEf);

        return booksToReturn;
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        var bookEf = await _repositoryManager.Book.GetBook(bookId, false);
        if (bookEf is null)
            throw new BookNotFoundException(bookId);

        _repositoryManager.Book.DeleteBook(bookEf);

        await _repositoryManager.SaveAsync();

        return true;
    }
}
