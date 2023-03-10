using Contracts;
using Contracts.EF;
using Entities.ModelsEf;
using Microsoft.EntityFrameworkCore;
using RepositoryEFCore.QueryHelper;
using Shared;
using Shared.Configuration;
using Shared.RequestParameters;

namespace RepositoryEFCore;

public class BookEfRepository : RepositoryBase<BookEf>, IBookEfRepository
{
    private readonly DatabaseOptions _databaseOptions;
    private readonly IIdGenerator _idGenerator;

    public BookEfRepository(RepositoryContext repositoryContext, DatabaseOptions databaseOptions, IIdGenerator idGenerator)
        : base(repositoryContext)
    {
        _databaseOptions = databaseOptions;
        _idGenerator = idGenerator;
    }

    //----------------------- CREATE ---------------------

    public void CreateBook(BookEf bookEf)
    {
        ArgumentNullException.ThrowIfNull(bookEf, nameof(bookEf));

        bookEf.Id = GetBookId();
        if (string.IsNullOrWhiteSpace(bookEf.Id))
            throw new Exception("Failed to create new ID.");

        Create(bookEf);
    }

    private string? GetBookId()
    {
        var booksWithCorrectPrefix = FindByCondition(b => b.Id.StartsWith(_databaseOptions.IdCharacterPrefix), false).ToList();

        var maxCurrentId = QueryHelperBookEf.FindMaxCurrentId(booksWithCorrectPrefix, _databaseOptions.IdCharacterPrefix);

        return _idGenerator.GenerateId(maxCurrentId);
    }

    //----------------------- DELETE ---------------------

    public void DeleteBook(BookEf bookEf)
    {
        Delete(bookEf);
    }

    //----------------------- READ ---------------------

    public async Task<IEnumerable<BookEf>> GetBooks(ReadBooksRequest readBooksRequest, bool trackChanges)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException($"{nameof(ReadBooksRequest)} cannot be null.");

        return readBooksRequest.HasFilters
            ? await GetBooksByCondition(readBooksRequest, trackChanges)
            : await GetAllBooks(readBooksRequest, trackChanges);
    }

    private async Task<IEnumerable<BookEf>> GetBooksByCondition(ReadBooksRequest readBooksRequest, bool trackChanges)
    {
        var findExpression = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        var foundBooks = FindByCondition(findExpression, trackChanges);

        if (readBooksRequest.SortResult)
            foundBooks = QueryHelperBookEf.OrderBooksBy(foundBooks, readBooksRequest);

        return await GetPaginatedQuery(readBooksRequest.BookParameters, foundBooks).ToListAsync();
    }

    private async Task<IEnumerable<BookEf>> GetAllBooks(ReadBooksRequest readBooksRequest, bool trackChanges)
    {
        var allBooks = FindAll(trackChanges);

        if (readBooksRequest.SortResult)
            allBooks = QueryHelperBookEf.OrderBooksBy(allBooks, readBooksRequest);

        return await GetPaginatedQuery(readBooksRequest.BookParameters, allBooks).ToListAsync();
    }

    public async Task<BookEf> GetBook(string bookId, bool trackChanges)
        => await FindByCondition(b => b.Id.Equals(bookId), trackChanges).FirstOrDefaultAsync();

    private static IQueryable<BookEf> GetPaginatedQuery(BookParameters bookParameters, IQueryable<BookEf> books)
        => books
            .Skip(QueryHelperBookEf.GetItemsToSkip(bookParameters.PageNumber, bookParameters.PageSize))
            .Take(bookParameters.PageSize);
}
