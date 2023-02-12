using BookApi.Configuration;
using BookApi.Data;
using Dapper;

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

        if (readBooksRequest.FilterByText)
        {
            var valueToFilterByPlaceHolder = GetPlaceHolder(nameof(readBooksRequest.FilterByTextValue));

            sqlQuery.QueryString.Append($" WHERE {fieldToSortBy} LIKE {valueToFilterByPlaceHolder}");

            sqlQuery.Parameters.Add(valueToFilterByPlaceHolder, CreateStringParameter(readBooksRequest));
        }

        if (readBooksRequest.FilterByDouble)
        {
            throw new NotImplementedException();
        }

        if (readBooksRequest.FilterByDate)
        {
            throw new NotImplementedException();
        }
    }

    private static string GetPlaceHolder(string variableName)
        => $"@{variableName}";

    private static object CreateStringParameter(ReadBooksRequest readBooksRequest)
        => $"%{readBooksRequest.FilterByTextValue}%";

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
