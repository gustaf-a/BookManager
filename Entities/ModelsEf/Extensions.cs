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

    public static BookEf ToBookEf(this BookDto bookDto)
        => new()
        {
            Id = bookDto.Id,
            Author = bookDto.Author,
            Description = bookDto.Description,
            Genre = bookDto.Genre,
            Price = bookDto.Price,
            PublishDate = bookDto.Publish_date,
            Title = bookDto.Title
        };
}
