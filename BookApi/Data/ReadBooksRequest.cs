namespace BookApi.Data;

/// <summary>
/// Contains parameters parsed from API request to the rest of the application.
/// </summary>
public class ReadBooksRequest
{
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
    public string FieldToSortBy { get; set; } = nameof(Book.Id);
    public FieldType Type { get; set; } = FieldType.Text;

    public bool HasFilters => FilterByValue || FilterByDate || FilterByPrice;

    public bool FilterByValue { get; set; } = false;
    public string ValueToFilterBy { get; set; } = string.Empty;

    public bool FilterByDate { get; set; } = false;
    public DatePrecision DatePrecisionToFilterOn { get; set; } = DatePrecision.None;
    public DateOnly DateToFilterOn { get; set; } = DateOnly.MinValue;

    public bool FilterByPrice { get; set; } = false;
    public double PriceExact { get; set; } = double.MinValue;
    public double PriceMin { get; set; } = double.MinValue;
    public double PriceMax { get; set; } = double.MaxValue;

    public bool PriceIsRanged => PriceMin > double.MinValue || PriceMax < double.MaxValue;
}
