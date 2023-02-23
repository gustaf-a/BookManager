using Shared.RequestParameters;

namespace Shared;

/// <summary>
/// Contains parameters parsed from API request to the rest of the application.
/// </summary>
public class ReadBooksRequest
{
    public BookParameters BookParameters { get; set; } = new BookParameters();

    public enum FieldType
    {
        Text,
        Numeric,
        Date
    }

    public enum DatePrecision
    {
        None,
        Year,
        Month,
        Day
    }

    public bool SortResult { get; set; } = true;
    public string SortResultByField { get; set; } = nameof(Book.Id);
    public FieldType SortResultByFieldType { get; set; } = FieldType.Text;

    public bool HasFilters => FilterByDate || FilterByDouble || FilterByText;

    public bool FilterByText => !string.IsNullOrWhiteSpace(FilterByTextValue);
    public string FilterByTextValue { get; set; } = string.Empty;

    public bool FilterByDouble => FilterByDoubleValue > double.MinValue;
    public double FilterByDoubleValue { get; set; } = double.MinValue;
    public double FilterByDoubleValue2 { get; set; } = double.MinValue;
    public bool FilterByDoubleRange => FilterByDoubleValue > double.MinValue && FilterByDoubleValue2 > double.MinValue;

    public bool FilterByDate => FilterByDatePrecision != DatePrecision.None;
    public DatePrecision FilterByDatePrecision { get; set; } = DatePrecision.None;
    public DateOnly FilterByDateValue { get; set; } = DateOnly.MinValue;

    public static DatePrecision FindDatePrecision(int year, int month, int day)
    {
        if (day > int.MinValue)
            return DatePrecision.Day;

        if (month > int.MinValue)
            return DatePrecision.Month;

        if (year > int.MinValue)
            return DatePrecision.Year;

        return DatePrecision.None;
    }
}
