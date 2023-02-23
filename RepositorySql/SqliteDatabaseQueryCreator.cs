using Entities.ModelsSql;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Configuration;
using Shared.RequestParameters;

namespace RepositorySql;

/// <summary>
/// Used to build parametrized SQL query strings to adhere to best security practices.
/// </summary>
public class SqliteDatabaseQueryCreator : IDatabaseQueryCreator
{
    private readonly DatabaseOptions _databaseOptions;

    private readonly string _booksTableName;

    private readonly int _idNumbersStartPosition;

    public SqliteDatabaseQueryCreator(IOptions<DatabaseOptions> options)
    {
        _databaseOptions = options.Value;

        if (_databaseOptions == null)
            throw new Exception("Unable to find DatabaseOptions");

        if (string.IsNullOrWhiteSpace(_databaseOptions.BooksTableName))
            throw new Exception("Unable to find BooksTableName");

        if (_databaseOptions.IdCharacterPrefixLength != 1)
            throw new NotSupportedException($"Values other than 1 for {nameof(_databaseOptions.IdCharacterPrefixLength)} currently not supported by SQL queries.");

        _booksTableName = _databaseOptions.BooksTableName;

        _idNumbersStartPosition = _databaseOptions.IdCharacterPrefixLength + 1;
    }

    // --------------------- CREATE ------------------------------------

    /// <summary>
    /// Creates SQL Query using parameters to create new object in database.
    /// 
    /// Example sql query:
    /// INSERT INTO books(id,author,title,genre,price,publish_date,description)
    /// VALUES (@Id,@Author,@Title,@Genre,@Price,@Publish_date,@Description);
    /// </summary>
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
        sqlQuery.QueryString.Append(propertyNames.GetPlaceHolderList());
        sqlQuery.QueryString.Append(");");

        sqlQuery.AddPlaceholderParameters(bookSqlite.GetProperties(includeDefaultValues: true, includeId: true));

        return sqlQuery;
    }

    // --------------------- READ ------------------------------------

    /// <summary>
    /// Creates SQL Query using parameters to read objects from database.
    /// 
    /// Example sql query:
    ///  SELECT * FROM books ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT 20;
    /// </summary>
    public SqlQuery Read(ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest == null)
            throw new ArgumentNullException($"{nameof(ReadBooksRequest)} cannot be null.");

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"SELECT * FROM {_booksTableName}");

        AddFiltersToQuery(sqlQuery, readBooksRequest);

        AddSortingAndPagingToQuery(sqlQuery, readBooksRequest);

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

    /// <summary>
    /// Appends SQL query for filtering different columns by text-value to SqlQuery object.
    /// 
    /// Example:
    ///  WHERE id LIKE @FilterByTextValue
    /// </summary>
    private static void FilterByText(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        var sortResultByField = readBooksRequest.SortResultByField.ToBookSqliteName();

        var filterByTextValuePlaceholder = nameof(readBooksRequest.FilterByTextValue).GetPlaceHolder();

        sqlQuery.QueryString.Append($" WHERE {sortResultByField} LIKE {filterByTextValuePlaceholder}");

        sqlQuery.Parameters.Add(filterByTextValuePlaceholder, SurroundStringWith(readBooksRequest.FilterByTextValue, "%"));
    }

    /// <summary>
    /// Returns value surrounded by another value.
    /// 
    /// Example with surroundValue "%": "SearchTerm" -> "%SearchTerm%" 
    /// </summary>
    private static object SurroundStringWith(string value, string surroundWithValue)
        => $"{surroundWithValue}{value}{surroundWithValue}";

    /// <summary>
    /// Appends SQL query for filtering different columns by double-value to SqlQuery object.
    /// 
    /// Example:
    ///  WHERE price = @FilterByDoubleValue
    /// 
    /// Example2:
    ///  WHERE price BETWEEN @FilterByDoubleValue AND @FilterByDoubleValue2
    /// </summary>
    private static void FilterByDouble(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        var sortResultByField = readBooksRequest.SortResultByField.ToBookSqliteName();

        var filterByDoubleValuePlaceholder = nameof(readBooksRequest.FilterByDoubleValue).GetPlaceHolder();

        sqlQuery.Parameters.Add(filterByDoubleValuePlaceholder, readBooksRequest.FilterByDoubleValue);

        if (readBooksRequest.FilterByDoubleRange)
        {
            var filterByDoubleValue2Placeholder = nameof(readBooksRequest.FilterByDoubleValue2).GetPlaceHolder();

            sqlQuery.Parameters.Add(filterByDoubleValue2Placeholder, readBooksRequest.FilterByDoubleValue2);

            sqlQuery.QueryString.Append($" WHERE {sortResultByField} BETWEEN {filterByDoubleValuePlaceholder} AND {filterByDoubleValue2Placeholder}");
        }
        else
            sqlQuery.QueryString.Append($" WHERE {sortResultByField} = {filterByDoubleValuePlaceholder}");
    }

    /// <summary>
    /// Appends SQL query for filtering different columns by date-value to SqlQuery object.
    /// 
    /// Example:
    ///  WHERE substring(publish_date,1,7) = substring('2012-04-01',1,7)
    /// </summary>
    private static void FilterByDate(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        if (readBooksRequest.FilterByDatePrecision == ReadBooksRequest.DatePrecision.None)
            return;

        var fieldToSortBy = readBooksRequest.SortResultByField.ToBookSqliteName();

        var datePrecisionLength = readBooksRequest.FilterByDatePrecision.ToSubstringLength();

        var dateOnlyString = readBooksRequest.FilterByDateValue.GetConvertedDateOnlyValue();

        // Possible values coming from DateOnly are limited so no need for parameters
        sqlQuery.QueryString.Append($" WHERE substring({fieldToSortBy.ToLower()},1,{datePrecisionLength}) = substring('{dateOnlyString}',1,{datePrecisionLength})");
    }

    /// <summary>
    /// Appends SQL query for sorting by different columns and paging results, skipping pages and limiting result size.
    /// 
    /// Example:
    ///  WHERE id NOT IN ( SELECT id FROM books ORDER BY description ASC LIMIT 10 ) ORDER BY description ASC LIMIT 10
    ///
    ///  
    /// Example2:
    ///   ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT 20
    /// </summary>
    private void AddSortingAndPagingToQuery(SqlQuery sqlQuery, ReadBooksRequest readBooksRequest)
    {
        var sortingQuery = GetSortingQueryString(readBooksRequest);

        var itemsToSkip = GetItemsToSkip(readBooksRequest.BookParameters.PageNumber, readBooksRequest.BookParameters.PageSize);

        if (itemsToSkip > 0)
            sqlQuery.QueryString.Append($" WHERE id NOT IN ( SELECT id FROM books {sortingQuery} LIMIT {itemsToSkip})");

        if(!string.IsNullOrEmpty(sortingQuery))
            sqlQuery.QueryString.Append($" {sortingQuery}");

        sqlQuery.QueryString.Append($" LIMIT {readBooksRequest.BookParameters.PageSize}");
    }

    public static int GetItemsToSkip(int pageNumber, int pageSize)
        => (pageNumber - 1) * pageSize;

    private string GetSortingQueryString(ReadBooksRequest readBooksRequest)
    {
        if (!readBooksRequest.SortResult)
            return string.Empty;

        return nameof(Book.Id).Equals(readBooksRequest.SortResultByField)
                    ? $"ORDER BY CAST(SUBSTRING(id,{_idNumbersStartPosition},{_databaseOptions.IdNumberMaxLength}) AS NUMERIC)"
                    : $"ORDER BY {readBooksRequest.SortResultByField.ToBookSqliteName()}";
    }

    // --------------------- UPDATE ------------------------------------

    /// <summary>
    /// Creates SQL query for updating a book. Ignores fields with default values.
    /// 
    /// Example:
    ///  UPDATE books SET author = @Author, title = @Title, price = @Price, publish_date = @Publish_date WHERE id = @Id;
    /// </summary>
    public SqlQuery Update(Book book)
    {
        if (book is null)
            throw new ArgumentNullException($"{nameof(book)} cannot be null.");

        if (string.IsNullOrWhiteSpace(book.Id))
            throw new ArgumentNullException($"{nameof(book.Id)} cannot be null.");

        var bookSqlite = book.ToBookSqlite();

        var propertiesToUpdate = bookSqlite.GetProperties();

        if (!propertiesToUpdate.Any())
            throw new Exception("Failed to find properties to update");

        var updatePropertyStrings = CreateUpdatePropertyStrings(propertiesToUpdate);

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"UPDATE {_booksTableName} SET");

        sqlQuery.QueryString.Append($" {string.Join(", ", updatePropertyStrings)}");

        sqlQuery.AddPlaceholderParameters(propertiesToUpdate);

        var idPlaceHolder = nameof(BookSqlite.Id).GetPlaceHolder();

        sqlQuery.QueryString.Append($" WHERE {nameof(BookSqlite.Id).ToLower()} = {idPlaceHolder};");

        sqlQuery.Parameters.Add(idPlaceHolder, book.Id);

        return sqlQuery;
    }

    /// <summary>
    /// Creates update parameter strings for different properties and joins into one string.
    /// 
    /// Example:
    /// author = @Author, title = @Title, price = @Price, publish_date = @Publish_date
    /// </summary>
    private static List<string> CreateUpdatePropertyStrings(Dictionary<string, object> propertiesToUpdate)
        => propertiesToUpdate.Select(p => $"{p.Key.ToLower()} = {p.Key.GetPlaceHolder()}").ToList();

    // --------------------- DELETE ------------------------------------

    /// <summary>
    /// Creates SQL query for deleting a book from a table by id
    /// 
    /// Example:
    ///  DELETE FROM books WHERE id=@Id;
    /// </summary>
    public SqlQuery Delete(string bookId)
    {
        if (string.IsNullOrWhiteSpace(bookId))
            throw new ArgumentNullException($"{nameof(bookId)} cannot be null.");

        var idPlaceHolder = nameof(BookSqlite.Id).GetPlaceHolder();

        var sqlQuery = new SqlQuery();

        sqlQuery.QueryString.Append($"DELETE FROM {_booksTableName} WHERE id={idPlaceHolder};");

        sqlQuery.Parameters.Add(idPlaceHolder, bookId);

        return sqlQuery;
    }

    // --------------------- GET VAlUE ------------------------------------

    /// <summary>
    /// Creates SQL query for getting values from tables.
    /// 
    /// Example:
    ///  SELECT id FROM books WHERE CAST(SUBSTRING(id, 2) AS UNSIGNED) = (SELECT MAX(CAST(SUBSTRING(id, 2) AS UNSIGNED)) FROM books);
    /// </summary>
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

        sqlQuery.QueryString.Append(" = (SELECT MAX(");
        sqlQuery.QueryString.Append(GetCastSubstringQuery(getValueRequest));
        sqlQuery.QueryString.Append(')');

        sqlQuery.QueryString.Append($" FROM {_booksTableName}");
        sqlQuery.QueryString.Append(");");

        return sqlQuery;
    }

    // Variable source is internal, so no need for parameters for SQL Injection protection
    private static string GetCastSubstringQuery(GetValueRequest getValueRequest)
    {
        return $"CAST(SUBSTRING({getValueRequest.ColumnName.ToLower()}, {getValueRequest.IgnoreFirstCharacters + 1}) AS UNSIGNED)";
    }
}
