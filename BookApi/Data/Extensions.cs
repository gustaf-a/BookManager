using System.Globalization;

namespace BookApi.Data;

internal static class Extensions
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

        try
        {
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
        catch (Exception)
        {
            return null;
        }
    }
}
