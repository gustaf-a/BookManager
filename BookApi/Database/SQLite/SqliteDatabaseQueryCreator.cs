using BookApi.Configuration;
using BookApi.Data;
using System.Globalization;

namespace BookApi.Database.SQLite;

/// <summary>
/// Used to build parametrized SQL query strings to adhere to best security practices.
/// </summary>
public class SqliteDatabaseQueryCreator : IDatabaseQueryCreator
{
    private readonly DatabaseOptions _databaseOptions;

    private readonly string _booksTableName;

    private readonly int _idNumbersStartPosition;

    public SqliteDatabaseQueryCreator(IConfiguration configuration)
    {
        _databaseOptions = configuration.GetSection(DatabaseOptions.Database).Get<DatabaseOptions>();

        if (_databaseOptions == null)
            throw new Exception("Unable to find DatabaseOptions");

        if (string.IsNullOrWhiteSpace(_databaseOptions.BooksTableName))
            throw new Exception("Unable to find BooksTableName");

        _booksTableName = _databaseOptions.BooksTableName;

        _idNumbersStartPosition = _databaseOptions.IdCharacterPrefixLength + 1;
    }

    public string Create(Book book)
    {
        throw new NotImplementedException();
    }

    public string Delete(Book book)
    {
        throw new NotImplementedException();
    }

    public SqlQuery Read(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException($"{nameof(ReadBooksRequest)} cannot be null.");

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"SELECT * FROM {_booksTableName}");

        AddFiltersToQuery(sqlQuery, readBooksRequest);

        AddSortingToQuery(sqlQuery, readBooksRequest);

        sqlQuery.QueryString.Append(';');

        return sqlQuery;
    }

    private static void AddFiltersToQuery(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest.FilterByText)
            FilterByText(sqlQuery, readBooksRequest);

        if (readBooksRequest.FilterByDouble)
            FilterByDouble(sqlQuery, readBooksRequest);

        if (readBooksRequest.FilterByDate)
            FilterByDate(sqlQuery, readBooksRequest);

        return;
    }

    private static void FilterByText(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        var sortResultByField = readBooksRequest.SortResultByField.ToBookSqliteName();

        var filterByTextValuePlaceholder = GetPlaceHolder(nameof(readBooksRequest.FilterByTextValue));

        sqlQuery.QueryString.Append($" WHERE {sortResultByField} LIKE {filterByTextValuePlaceholder}");

        sqlQuery.Parameters.Add(filterByTextValuePlaceholder, CreateStringParameter(readBooksRequest));
    }

    private static string GetPlaceHolder(string variableName)
        => $"@{variableName}";

    private static object CreateStringParameter(ReadBooksRequest readBooksRequest)
        => $"%{readBooksRequest.FilterByTextValue}%";

    private static void FilterByDouble(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        var sortResultByField = readBooksRequest.SortResultByField.ToBookSqliteName();

        var filterByDoubleValuePlaceholder = GetPlaceHolder(nameof(readBooksRequest.FilterByDoubleValue));

        sqlQuery.Parameters.Add(filterByDoubleValuePlaceholder, readBooksRequest.FilterByDoubleValue);

        if (readBooksRequest.FilterByDoubleRange)
        {
            var filterByDoubleValue2Placeholder = GetPlaceHolder(nameof(readBooksRequest.FilterByDoubleValue2));

            sqlQuery.Parameters.Add(filterByDoubleValue2Placeholder, readBooksRequest.FilterByDoubleValue2);

            sqlQuery.QueryString.Append($" WHERE {sortResultByField} BETWEEN {filterByDoubleValuePlaceholder} AND {filterByDoubleValue2Placeholder}");
        }
        else
            sqlQuery.QueryString.Append($" WHERE {sortResultByField} = {filterByDoubleValuePlaceholder}");
    }

    // Possible values coming from DateOnly are very limited so no need for parameters
    private static void FilterByDate(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest.FilterByDatePrecision == ReadBooksRequest.DatePrecision.None)
            return;

        var fieldToSortBy = readBooksRequest.SortResultByField.ToBookSqliteName();

        var datePrecisionLength = readBooksRequest.FilterByDatePrecision.ToSubstringLength();

        var dateOnlyString = readBooksRequest.FilterByDateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        sqlQuery.QueryString.Append($" WHERE substring({fieldToSortBy.ToLower()},1,{datePrecisionLength}) = substring('{dateOnlyString}',1,{datePrecisionLength})");
    }

    private void AddSortingToQuery(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        if (!readBooksRequest.SortResult)
            return;

        if (readBooksRequest.SortResultByField == nameof(Book.Id))
        {
            sqlQuery.QueryString.Append($" ORDER BY CAST(SUBSTRING(id,{_idNumbersStartPosition},{_databaseOptions.IdNumberMaxLength}) AS NUMERIC)");
            return;
        }

        sqlQuery.QueryString.Append($" ORDER BY {readBooksRequest.SortResultByField.ToBookSqliteName()} ASC");
    }

    public string Update(Book book)
    {
        throw new NotImplementedException();
    }
}
