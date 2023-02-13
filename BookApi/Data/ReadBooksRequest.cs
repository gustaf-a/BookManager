﻿namespace BookApi.Data;

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
    public string SortResultByField { get; set; } = nameof(Book.Id);
    public FieldType SortResultByFieldType { get; set; } = FieldType.Text;

    public bool FilterByText { get; set; } = false;
    public string FilterByTextValue { get; set; } = string.Empty;

    public bool FilterByDouble { get; set; } = false;
    public double FilterByDoubleValue { get; set; } = double.MinValue;
    public double FilterByDoubleValue2 { get; set; } = double.MinValue;
    public bool FilterByDoubleRange => FilterByDoubleValue > double.MinValue && FilterByDoubleValue2 > double.MinValue;

    public bool FilterByDate { get; set; } = false;
    public DatePrecision FilterByDatePrecision { get; set; } = DatePrecision.None;
    public DateOnly FilterByDateValue { get; set; } = DateOnly.MinValue;

    public static ReadBooksRequest.DatePrecision FindDatePrecision(int year, int month, int day)
    {
        if (day > int.MinValue)
            return ReadBooksRequest.DatePrecision.Day;

        if (month > int.MinValue)
            return ReadBooksRequest.DatePrecision.Month;

        if (year > int.MinValue)
            return ReadBooksRequest.DatePrecision.Year;

        return ReadBooksRequest.DatePrecision.None;
    }
}
