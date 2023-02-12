using BookApi.Configuration;
using BookApi.Data;

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
        if (!readBooksRequest.HasFilters)
            return;

        var fieldToSortBy = readBooksRequest.FieldToSortBy.ToBookSqliteName();

        var fieldToSortByPlaceHolder = GetPlaceHolder(nameof(fieldToSortBy));

        sqlQuery.QueryString.Append($" WHERE {fieldToSortBy}={fieldToSortByPlaceHolder}");

        sqlQuery.Parameters.Add(fieldToSortByPlaceHolder, GetFormattedValueToFilterBy(readBooksRequest));
    }

    private static string GetPlaceHolder(string variableName)
        => $"@{variableName}";

    private static string GetFormattedValueToFilterBy(ReadBooksRequest readBooksRequest)
    {
        return readBooksRequest.Type switch
        {
            ReadBooksRequest.FieldType.Numeric => readBooksRequest.ValueToFilterBy,
            ReadBooksRequest.FieldType.Text => $"'{readBooksRequest.ValueToFilterBy}'",
            ReadBooksRequest.FieldType.Date => throw new NotImplementedException($"Filtering by value when {nameof(readBooksRequest.FieldToSortBy)} is {readBooksRequest.Type} not supported."),
            _ => throw new NotImplementedException($"ValueToFilterBy conversion not implemented for type {readBooksRequest.Type}."),
        };
    }

    private void AddSortingToQuery(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        if (!readBooksRequest.SortResult)
            return;

        if (readBooksRequest.FieldToSortBy == nameof(Book.Id))
        {
            sqlQuery.QueryString.Append($" ORDER BY CAST(SUBSTRING(id,{_idNumbersStartPosition},{_databaseOptions.IdNumberMaxLength}) AS NUMERIC)");
            return;
        }

        sqlQuery.QueryString.Append($" ORDER BY {readBooksRequest.FieldToSortBy.ToBookSqliteName()} ASC");
    }

    public string Update(Book book)
    {
        throw new NotImplementedException();
    }
}
