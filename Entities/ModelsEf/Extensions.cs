namespace Entities.ModelsEf;

public static class Extensions
{
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
