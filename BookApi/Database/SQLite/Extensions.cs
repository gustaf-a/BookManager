﻿using BookApi.Data;
using System.Globalization;

namespace BookApi.Database.SQLite;

internal static class Extensions
{
    //This is unlikely to change and can be stored as a constant instead of in configurations.
    private const string DateFormat = "yyyy-MM-dd";

    public static IEnumerable<Book> ToBooks(this IEnumerable<BookSqlite> values)
        => values.Select(b => b.ToBook());

    public static Book ToBook(this BookSqlite value)
        => new()
        {
            Id = value.Id,
            Author = value.Author,
            Description = value.Description,
            Genre = value.Genre,
            Price = value.Price,
            PublishDate = DateOnly.ParseExact(value.Publish_date, DateFormat, CultureInfo.InvariantCulture),
            Title = value.Title
        };

    public static BookSqlite ToBookSqlite(this Book book)
        => new()
        {
            Id = book.Id,
            Author = book.Author,
            Description = book.Description,
            Genre = book.Genre,
            Price = book.Price,
            Publish_date = GetConvertedDateOnlyValue(book.PublishDate),
            Title = book.Title
        };

    private static string GetConvertedDateOnlyValue(DateOnly publishDate)
        => publishDate == DateOnly.MinValue ? null : publishDate.ToString(DateFormat, CultureInfo.InvariantCulture);

    public static string ToBookSqliteName(this string bookName)
        => bookName switch
        {
            nameof(Book.Id) => nameof(BookSqlite.Id).ToLower(),
            nameof(Book.Author) => nameof(BookSqlite.Author).ToLower(),
            nameof(Book.Description) => nameof(BookSqlite.Description).ToLower(),
            nameof(Book.Genre) => nameof(BookSqlite.Genre).ToLower(),
            nameof(Book.Price) => nameof(BookSqlite.Price).ToLower(),
            nameof(Book.PublishDate) => nameof(BookSqlite.Publish_date).ToLower(),
            nameof(Book.Title) => nameof(BookSqlite.Title).ToLower(),
            _ => throw new NotImplementedException($"Property name conversion not implemented for {bookName}")
        };

    public static int ToSubstringLength(this ReadBooksRequest.DatePrecision datePrecision)
        => datePrecision switch
        {
            ReadBooksRequest.DatePrecision.None => 0,
            ReadBooksRequest.DatePrecision.Year => 4,
            ReadBooksRequest.DatePrecision.Month => 7,
            ReadBooksRequest.DatePrecision.Day => 10,
            _ => throw new NotImplementedException($"Substring length not found for DatePrecision {datePrecision}.")
        };

    public static void AddIfNotDefault(this Dictionary<string, object> dictionary, string stringValue, string key)
    {
        if (!string.IsNullOrEmpty(stringValue))
            dictionary.Add(key, stringValue);
    }

    public static void AddIfNotDefault(this Dictionary<string, object> dictionary, double doubleValue, string key)
    {
        if (doubleValue > double.MinValue)
            dictionary.Add(key, doubleValue);
    }

    public static void AddPlaceholderParameters(this SqlQuery sqlQuery, Dictionary<string, object> properties)
    {
        foreach (var property in properties)
            sqlQuery.Parameters.Add(GetPlaceHolder(property.Key), property.Value);
    }

    public static string GetPlaceHolderList(this IEnumerable<string> variableNames)
        => string.Join(',', variableNames.Select(vn => vn.GetPlaceHolder()));

    public static string GetPlaceHolder(this string variableName)
        => $"@{variableName}";
}
