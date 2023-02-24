using Entities.ModelsEf;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.RequestParameters;
using System.Linq.Expressions;

namespace RepositoryEFCore.QueryHelper;

public static class QueryHelperBookEf
{
    public static Expression<Func<BookEf, bool>> CreateFindExpression(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException(nameof(readBooksRequest));

        if (readBooksRequest.FilterByText)
            return CreateFindByTextExpression(readBooksRequest);

        if (readBooksRequest.FilterByDate)
            return CreateFindByDateExpression(readBooksRequest);

        if (readBooksRequest.FilterByDouble)
            return CreateFindByDoubleExpression(readBooksRequest);

        throw new NotImplementedException($"{nameof(ReadBooksRequest)} configuration not supported.");
    }

    private static Expression<Func<BookEf, bool>> CreateFindByTextExpression(ReadBooksRequest readBooksRequest)
    {
        return readBooksRequest.SortResultByField switch
        {
            nameof(Book.Id) => b => EF.Functions.Like(b.Id, $"%{readBooksRequest.FilterByTextValue}%"),
            nameof(Book.Author) => b => EF.Functions.Like(b.Author, $"%{readBooksRequest.FilterByTextValue}%"),
            nameof(Book.Description) => b => EF.Functions.Like(b.Description, $"%{readBooksRequest.FilterByTextValue}%"),
            nameof(Book.Genre) => b => EF.Functions.Like(b.Genre, $"%{readBooksRequest.FilterByTextValue}%"),
            nameof(Book.Title) => b => EF.Functions.Like(b.Title, $"%{readBooksRequest.FilterByTextValue}%"),

            nameof(Book.PublishDate) => throw new Exception($"{nameof(Book.PublishDate)} not treated as text. Please use {nameof(readBooksRequest.FilterByDate)}"),
            nameof(Book.Price) => throw new Exception($"{nameof(Book.Price)} not treated as text. Please use {nameof(readBooksRequest.FilterByDouble)}"),

            _ => throw new NotImplementedException($"Find expression for {readBooksRequest.FilterByTextValue} not implemented."),
        };
    }

    private static Expression<Func<BookEf, bool>> CreateFindByDateExpression(ReadBooksRequest readBooksRequest)
    {
        var filterByDate = readBooksRequest.FilterByDateValue.ToDateOnlyString();

        if (string.IsNullOrWhiteSpace(filterByDate))
            throw new Exception($"Failed to convert from DateOnly to string: {readBooksRequest.FilterByDateValue}");

        var subStringLength = readBooksRequest.FilterByDatePrecision.ToSubstringLength();

        var filterByDateSubstring = filterByDate[..subStringLength];

        return b => b.PublishDate.StartsWith(filterByDateSubstring);
    }

    private static Expression<Func<BookEf, bool>> CreateFindByDoubleExpression(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest.FilterByDoubleRange)
            return b => readBooksRequest.FilterByDoubleValue <= b.Price
                        && b.Price <= readBooksRequest.FilterByDoubleValue2;

        return b => b.Price == readBooksRequest.FilterByDoubleValue;
    }

    public static IQueryable<BookEf> OrderBooksBy(IQueryable<BookEf> books, ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException(nameof(readBooksRequest));

        if (books == null)
            throw new ArgumentNullException(nameof(books));

        if (!readBooksRequest.SortResult
            || books.Count() <= 1)
            return books;

        return readBooksRequest.SortResultByField switch
        {
            nameof(Book.Id) => OrderById(books),
            nameof(Book.Author) => books.OrderBy(b => b.Author),
            nameof(Book.Description) => books.OrderBy(b => b.Description),
            nameof(Book.Genre) => books.OrderBy(b => b.Genre),
            nameof(Book.Price) => books.OrderBy(b => b.Price),
            nameof(Book.PublishDate) => books.OrderBy(b => b.PublishDate),
            nameof(Book.Title) => books.OrderBy(b => b.Title),

            _ => throw new NotImplementedException($"Sorting by {readBooksRequest.SortResultByField} not implemented."),
        };
    }

    /// <summary>
    /// Orders by numbers and then orders by first character
    /// </summary>
    private static IQueryable<BookEf> OrderById(IQueryable<BookEf> books)
        => books.OrderBy(b => b.Id.Substring(2)).ThenBy(b => b.Id.Substring(1, 1));

    public static string FindMaxCurrentId(IEnumerable<BookEf> booksWithCorrectIdPrefix, string idCharacterPrefix)
    {
        if (booksWithCorrectIdPrefix == null
            || !booksWithCorrectIdPrefix.Any())
            return string.Empty;

        var bookWithMaxIdNumber = booksWithCorrectIdPrefix.OrderByDescending(
            b => GetNumberFromId(idCharacterPrefix, b)).First();

        return bookWithMaxIdNumber.Id ?? string.Empty;
    }

    private static int GetNumberFromId(string idCharacterPrefix, BookEf b)
        => int.Parse(b.Id.Substring(idCharacterPrefix.Length));

    /// <summary>
    /// Returns number of items to skip to get the items on the provided pageNumber.
    /// </summary>
    public static int GetItemsToSkip(int pageNumber, int pageSize)
        => (pageNumber - 1) * pageSize;
}
