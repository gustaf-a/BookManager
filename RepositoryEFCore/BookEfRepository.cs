﻿using Contracts;
using Contracts.EF;
using Entities.ModelsEf;
using RepositoryEFCore.QueryHelper;
using Shared;
using Shared.Configuration;

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

    public Task<IEnumerable<BookEf>> GetBooks(ReadBooksRequest readBooksRequest, bool trackChanges)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException($"{nameof(ReadBooksRequest)} cannot be null.");

        return readBooksRequest.HasFilters 
            ? GetBooksByCondition(readBooksRequest, trackChanges)
            : GetAllBooks(readBooksRequest, trackChanges);
    }

    private Task<IEnumerable<BookEf>> GetBooksByCondition(ReadBooksRequest readBooksRequest, bool trackChanges)
        => Task.FromResult(GetBooksByConditionInternal(readBooksRequest, trackChanges));

    private IEnumerable<BookEf> GetBooksByConditionInternal(ReadBooksRequest readBooksRequest, bool trackChanges)
    {
        var findExpression = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        var foundBooks = FindByCondition(findExpression, trackChanges);

        return readBooksRequest.SortResult
            ? QueryHelperBookEf.OrderBooksBy(foundBooks, readBooksRequest).ToList()
            : foundBooks.ToList();
    }

    private Task<IEnumerable<BookEf>> GetAllBooks(ReadBooksRequest readBooksRequest, bool trackChanges)
        => Task.FromResult(GetAllBooksInternal(readBooksRequest, trackChanges));

    private IEnumerable<BookEf> GetAllBooksInternal(ReadBooksRequest readBooksRequest, bool trackChanges)
    {
        var allBooks = FindAll(trackChanges);

        return readBooksRequest.SortResult
            ? QueryHelperBookEf.OrderBooksBy(allBooks, readBooksRequest).ToList()
            : allBooks.ToList();
    }
}
