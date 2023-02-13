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
}
