using System.Globalization;

namespace Shared;

public static class Extensions
{
    private const string DateFormatting = "yyyy-MM-dd";

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

    public static DateOnly ToDateOnly(this string publishDate)
        => string.IsNullOrWhiteSpace(publishDate)
            ? DateOnly.MinValue
            : DateOnly.ParseExact(publishDate, DateFormatting, CultureInfo.InvariantCulture);

    public static string ToDateOnlyString(this DateOnly publishDate)
        => DateOnly.MinValue.Equals(publishDate)
            ? string.Empty
            : publishDate.ToString(DateFormatting, CultureInfo.InvariantCulture);

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
