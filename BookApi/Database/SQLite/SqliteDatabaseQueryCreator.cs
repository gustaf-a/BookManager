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

        if (_databaseOptions.IdCharacterPrefixLength != 1)
            throw new NotSupportedException($"Values other than 1 for {nameof(_databaseOptions.IdCharacterPrefixLength)} currently not supported by SQL queries.");

        _booksTableName = _databaseOptions.BooksTableName;

        _idNumbersStartPosition = _databaseOptions.IdCharacterPrefixLength + 1;
    }

    private static string GetPlaceHolderList(IEnumerable<string> variableNames)
        => string.Join(',', variableNames.Select(vn => GetPlaceHolder(vn)));

    private static string GetPlaceHolder(string variableName)
        => $"@{variableName}";

    // --------------------- CREATE ------------------------------------

    public SqlQuery Create(Book book)
    {
        if (book == null)
            throw new ArgumentNullException($"{nameof(Book)} cannot be null.");

        if (string.IsNullOrWhiteSpace(book.Id))
            throw new ArgumentNullException($"Book {nameof(Book.Id)} cannot be null.");

        var bookSqlite = book.ToBookSqlite();

        var propertyNames = BookSqlite.GetPropertyNames();

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"INSERT INTO {_booksTableName}");
        sqlQuery.QueryString.Append("(id,author,title,genre,price,publish_date,description)");
        sqlQuery.QueryString.Append(" VALUES (");
        sqlQuery.QueryString.Append(GetPlaceHolderList(propertyNames));
        sqlQuery.QueryString.Append(')');
        sqlQuery.QueryString.Append(';');

        AddPropertiesAsParameters(bookSqlite, sqlQuery);

        return sqlQuery;
    }

    private static void AddPropertiesAsParameters(BookSqlite bookSqlite, SqlQuery sqlQuery)
    {
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Id)), bookSqlite.Id);
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Author)), bookSqlite.Author);
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Title)), bookSqlite.Title);
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Genre)), bookSqlite.Genre);
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Price)), bookSqlite.Price);
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Publish_date)), bookSqlite.Publish_date);
        sqlQuery.Parameters.Add(GetPlaceHolder(nameof(bookSqlite.Description)), bookSqlite.Description);
    }

    // --------------------- READ ------------------------------------

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

    // Possible values coming from DateOnly are limited so no need for parameters
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

    // --------------------- UPDATE ------------------------------------

    public SqlQuery Update(Book book, string bookId)
    {
        if (book is null)
            throw new ArgumentNullException($"{nameof(book)} cannot be null.");

        if (string.IsNullOrWhiteSpace(bookId))
            throw new ArgumentNullException($"{nameof(bookId)} cannot be null.");

        var bookSqlite = book.ToBookSqlite();

        var propertiesToUpdate = bookSqlite.GetProperties();

        if (!propertiesToUpdate.Any())
            throw new Exception("Failed to find properties to update");

        var updatePropertyStrings = CreateUpdatePropertyStrings(propertiesToUpdate);

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"UPDATE {_booksTableName} SET");

        sqlQuery.QueryString.Append($" {string.Join(", ", updatePropertyStrings)}");

        AddPlaceholderParameters(sqlQuery, propertiesToUpdate);

        var idPlaceHolder = GetPlaceHolder(nameof(BookSqlite.Id));

        sqlQuery.QueryString.Append($" WHERE {nameof(BookSqlite.Id).ToLower()} = {idPlaceHolder};");

        sqlQuery.Parameters.Add(idPlaceHolder, bookId);

        return sqlQuery;
    }

    private static List<string> CreateUpdatePropertyStrings(Dictionary<string, object> propertiesToUpdate)
        => propertiesToUpdate.Select(p => $"{p.Key.ToLower()} = {GetPlaceHolder(p.Key)}").ToList();

    // --------------------- DELETE ------------------------------------

    public SqlQuery Delete(string bookId)
    {
        if (string.IsNullOrWhiteSpace(bookId))
            throw new ArgumentNullException($"{nameof(bookId)} cannot be null.");

        var idPlaceHolder = GetPlaceHolder(nameof(BookSqlite.Id));

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"DELETE FROM books WHERE id={idPlaceHolder};");

        sqlQuery.Parameters.Add(idPlaceHolder, bookId);

        return sqlQuery;
    }

    // --------------------- GET VAlUE ------------------------------------

    public SqlQuery GetValueQuery(GetValueRequest getValueRequest)
    {
        if (getValueRequest == null)
            throw new ArgumentNullException($"{nameof(ReadBooksRequest)} cannot be null.");

        if (getValueRequest.GetMaxValue)
            return GetMaxValueQuery(getValueRequest);

        throw new NotImplementedException("Currently only GetMaxValue-queries supported.");
    }

    /// <summary>
    /// Returns the whole value, for example "B13" if 13 is highest value without prefix in column
    /// </summary>
    private SqlQuery GetMaxValueQuery(GetValueRequest getValueRequest)
    {
        var sqlQuery = new SqlQuery();

        if (!nameof(BookSqlite.Id).Equals(getValueRequest.ColumnName))
            throw new NotImplementedException($"Currently only GetMaxValue for {nameof(BookSqlite.Id)} supported.");

        sqlQuery.QueryString.Append($"SELECT {getValueRequest.ColumnName.ToLower()} FROM {_booksTableName}");

        sqlQuery.QueryString.Append($" WHERE {GetCastSubstringQuery(getValueRequest)}");

        sqlQuery.QueryString.Append(" = ");
        sqlQuery.QueryString.Append('(');

        sqlQuery.QueryString.Append($"SELECT MAX");
        sqlQuery.QueryString.Append('(');
        sqlQuery.QueryString.Append(GetCastSubstringQuery(getValueRequest));
        sqlQuery.QueryString.Append(')');

        sqlQuery.QueryString.Append($" FROM {_booksTableName}");
        sqlQuery.QueryString.Append(')');

        sqlQuery.QueryString.Append(';');

        return sqlQuery;
    }

    // Variable source is internal, so no need for parameters for SQL Injection protection
    private static string GetCastSubstringQuery(GetValueRequest getValueRequest)
    {
        return $"CAST(SUBSTRING({getValueRequest.ColumnName.ToLower()}, {getValueRequest.IgnoreFirstCharacters}) AS UNSIGNED)";
    }
}
