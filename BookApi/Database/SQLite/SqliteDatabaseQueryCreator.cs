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

        if (!readBooksRequest.HasFilters && !readBooksRequest.SortResult)
            return GetNoFiltersQuery();

        if (readBooksRequest.FilterByValue)
            return GetFilterByValueQuery(readBooksRequest);

        return GetSortedByFieldQuery(readBooksRequest);
    }

    private SqlQuery GetSortedByFieldQuery(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest.FieldToSortBy == nameof(Book.Id))
            return GetByIdWithSortingQuery();

        return new SqlQuery { QueryString = $"SELECT * FROM {_booksTableName} ORDER BY {readBooksRequest.FieldToSortBy.ToBookSqliteName()} ASC;" };
    }

    private SqlQuery GetByIdWithSortingQuery()
        => new() { QueryString = $"SELECT * FROM books ORDER BY CAST(SUBSTRING(id,{_idNumbersStartPosition},{_databaseOptions.IdNumberMaxLength}) AS NUMERIC);" };

    private SqlQuery GetNoFiltersQuery()
        => new() { QueryString = $"SELECT * FROM {_booksTableName};" };

    private SqlQuery GetFilterByValueQuery(ReadBooksRequest readBooksRequest)
    {
        var query = new SqlQuery();

        var fieldToSortBy = readBooksRequest.FieldToSortBy.ToBookSqliteName();

        var fieldToSortByPlaceHolder = GetPlaceHolder(nameof(fieldToSortBy));

        query.QueryString = $"SELECT * FROM {_booksTableName} WHERE {fieldToSortBy}={fieldToSortByPlaceHolder} ORDER BY {fieldToSortBy} ASC;";

        query.Parameters.Add(fieldToSortByPlaceHolder, GetFormattedValueToFilterBy(readBooksRequest));

        return query;
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

    public string Update(Book book)
    {
        throw new NotImplementedException();
    }
}
