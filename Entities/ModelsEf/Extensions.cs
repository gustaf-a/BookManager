using Shared;

namespace Entities.ModelsEf;

public static class Extensions
{
    public static IEnumerable<BookDto> ToBooksDto(this IEnumerable<BookEf> booksEf)
    {
        if (booksEf is null)
            return null;

        if (!booksEf.Any())
            return new List<BookDto>();

        return booksEf.Select(b => b.ToBookDto());
    }

    public static BookDto ToBookDto(this BookEf bookEf)
    {
        if (bookEf is null)
            return null;

        try
        {
            return new BookDto
            {
                Id = bookEf.Id,
                Author = bookEf.Author,
                Description = bookEf.Description,
                Genre = bookEf.Genre,
                Price = bookEf.Price,
                Publish_date = bookEf.PublishDate,
                Title = bookEf.Title
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    //Conforms to RepositorySql pattern and validates date.
    //To be replaced with better validation to allow direct BookDto->BookEf conversion. (2023-02-21)
    public static BookEf ToBookEf(this BookDto bookDto)
        => bookDto.ToBook().ToBookEf();

    public static BookEf ToBookEf(this Book book)
        => new()
        {
            Id = book.Id,
            Author = book.Author,
            Description = book.Description,
            Genre = book.Genre,
            Price = book.Price,
            PublishDate = book.PublishDate.GetConvertedDateOnlyValue(),
            Title = book.Title
        };

    public static void UpdateBookEf(this BookEf bookEf, BookEf bookEfUpdated)
    {
        if(NotDefault(bookEfUpdated.Author))
            bookEf.Author = bookEfUpdated.Author;

        if (NotDefault(bookEfUpdated.Description))
            bookEf.Description = bookEfUpdated.Description;

        if (NotDefault(bookEfUpdated.Genre))
            bookEf.Genre = bookEfUpdated.Genre;

        if (NotDefault(bookEfUpdated.Price))
            bookEf.Price = bookEfUpdated.Price;

        if (NotDefault(bookEfUpdated.PublishDate))
            bookEf.PublishDate = bookEfUpdated.PublishDate;

        if (NotDefault(bookEfUpdated.Title))
            bookEf.Title = bookEfUpdated.Title;
    }

    private static bool NotDefault(string? value)
        => !string.IsNullOrWhiteSpace(value);

    private static bool NotDefault(double value)
        => value > double.MinValue;
}
