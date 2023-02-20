using System.Globalization;

namespace Shared;

public static class Extensions
{
    public static DateOnly GetDateOnly(this ReadBooksRequest.DatePrecision datePrecision, int year, int month, int day)
    {
        if (datePrecision == ReadBooksRequest.DatePrecision.Day)
            return new DateOnly(year, month, day);

        if (datePrecision == ReadBooksRequest.DatePrecision.Month)
            return new DateOnly(year, month, 1);

        if (datePrecision == ReadBooksRequest.DatePrecision.Year)
            return new DateOnly(year, 1, 1);

        return DateOnly.MinValue;
    }

    public static Book ToBook(this BookDto bookDto)
    {
        if (bookDto is null)
            return null;

        return new Book
        {
            Id = bookDto.Id,
            Author = bookDto.Author,
            Description = bookDto.Description,
            Genre = bookDto.Genre,
            Price = bookDto.Price,
            PublishDate = string.IsNullOrWhiteSpace(bookDto.Publish_date) ? DateOnly.MinValue : DateOnly.ParseExact(bookDto.Publish_date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Title = bookDto.Title
        };
    }

    public static IEnumerable<BookDto> ToBooksDto(this IEnumerable<Book> books)
    {
        if (books is null)
            return null;

        if (!books.Any())
            return new List<BookDto>();

        return books.Select(b => b.ToBookDto());
    }

    public static BookDto ToBookDto(this Book book)
    {
        if (book is null)
            return null;

        try
        {
            return new BookDto
            {
                Id = book.Id,
                Author = book.Author,
                Description = book.Description,
                Genre = book.Genre,
                Price = book.Price,
                Publish_date = book.PublishDate.GetConvertedDateOnlyValue(),
                Title = book.Title
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string? GetConvertedDateOnlyValue(this DateOnly publishDate)
        => DateOnly.MinValue.Equals(publishDate)
            ? null
            : publishDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static int ToSubstringLength(this ReadBooksRequest.DatePrecision datePrecision)
    => datePrecision switch
    {
        ReadBooksRequest.DatePrecision.None => 0,
        ReadBooksRequest.DatePrecision.Year => 4,
        ReadBooksRequest.DatePrecision.Month => 7,
        ReadBooksRequest.DatePrecision.Day => 10,
        _ => throw new NotImplementedException($"Substring length not found for DatePrecision {datePrecision}.")
    };
}
