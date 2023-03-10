using Entities.ModelsSql;
using Microsoft.Extensions.Options;
using RepositorySql;
using Shared;
using Shared.Configuration;
using System.Globalization;

namespace RepositorySqlTests;

public class SqliteDatabaseQueryCreatorTests
{
    private readonly IDatabaseQueryCreator _queryCreator;
    private readonly IOptions<DatabaseOptions> _databaseOptions;

    public SqliteDatabaseQueryCreatorTests()
    {
        var databaseOptions = new DatabaseOptions
        {
            BooksTableName = "books",
            IdCharacterPrefix = "B",
            IdNumberMaxLength = 9
        };

        _databaseOptions = Options.Create(databaseOptions);

        _queryCreator = new SqliteDatabaseQueryCreator(_databaseOptions);
    }

    // --------------------- CREATE ------------------------------------

    [Fact]
    public void Create_ThrowsException_When_Book_Null()
    {
        _queryCreator.Invoking(y => y.Create(null))
            .Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'bookSqlite cannot be null.')");
    }

    [Fact]
    public void Create_ThrowsException_When_BookId_Null()
    {
        // Arrange
        var book = new BookSqlite
        {
            //Id = null,
            Author = "Testson, Tester",
            Title = "How to test",
            Genre = "Testing",
            Price = 4.99,
            Description = "The most testing book you'll ever read.",
            Publish_date = "2023-02-11"
        };

        _queryCreator.Invoking(y => y.Create(book))
            .Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'Book Id cannot be null.')");
    }

    [Fact]
    public void Create_ReturnsCreateBookQuery()
    {
        // Arrange
        var book = new BookSqlite
        {
            Id = "B2020",
            Author = "Testson, Tester",
            Title = "How to test",
            Genre = "Testing",
            Price = 4.99,
            Description = "The most testing book you'll ever read.",
            Publish_date = "2023-02-11"
        };

        var expectedQuery = $"INSERT INTO books(id,author,title,genre,price,publish_date,description) " +
            $"VALUES (@Id,@Author,@Title,@Genre,@Price,@Publish_date,@Description);";

        // Act
        var sqlQuery = _queryCreator.Create(book);

        // Assert
        sqlQuery.QueryString.ToString().Should().Be(expectedQuery);

        var parameters = sqlQuery.Parameters;

        parameters.Count.Should().Be(7);

        parameters["@Id"].Should().Be(book.Id);
        parameters["@Author"].Should().Be(book.Author);
        parameters["@Title"].Should().Be(book.Title);
        parameters["@Genre"].Should().Be(book.Genre);
        parameters["@Price"].Should().Be(book.Price);
        parameters["@Description"].Should().Be(book.Description);
        parameters["@Publish_date"].Should().Be("2023-02-11");
    }

    // --------------------- READ ------------------------------------

    [Fact]
    public void Read_ThrowsException_When_ReadBooksRequest_Null()
    {
        _queryCreator.Invoking(y => y.Read(null))
            .Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'ReadBooksRequest cannot be null.')");
    }

    [Fact]
    public void Read_ReturnsSelectAllBooksQuery_WithoutParameters_WhenDefault_ReadBooksRequest()
    {
        // Arrange
        var expectedQuery = "SELECT * FROM books LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = false
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    [Fact]
    public void Read_ReturnsSelectAllBooksQuery_ById_WithoutParameters_WithSortingByNumberInId()
    {
        // Arrange
        //9 is for IdNumberMaxLength set in appsettings
        //2 is for starting point when IdCharacterPrefixLength is Length 1 in appsettings
        var expectedQuery = "SELECT * FROM books ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Id),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    [Theory]
    [InlineData(nameof(Book.Author), nameof(BookSqlite.Author), ReadBooksRequest.FieldType.Text)]
    [InlineData(nameof(Book.Title), nameof(BookSqlite.Title), ReadBooksRequest.FieldType.Text)]
    [InlineData(nameof(Book.Price), nameof(BookSqlite.Price), ReadBooksRequest.FieldType.Numeric)]
    [InlineData(nameof(Book.Description), nameof(BookSqlite.Description), ReadBooksRequest.FieldType.Text)]
    [InlineData(nameof(Book.Genre), nameof(BookSqlite.Genre), ReadBooksRequest.FieldType.Text)]
    [InlineData(nameof(Book.PublishDate), nameof(BookSqlite.Publish_date), ReadBooksRequest.FieldType.Date)]
    public void Read_ReturnsSelectAllBooksQuery_BySqliteFieldToSortBy_WithoutParameters(string fieldToSortBy, string sqliteFieldToSortBy, ReadBooksRequest.FieldType fieldType)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = fieldToSortBy,
            SortResultByFieldType = fieldType
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    [Fact]
    public void Read_ReturnsSelectAllBooksQuery_WhereLike_ValueToFilterBy_WhenSortBy_Id()
    {
        // Arrange
        var expectedQuery = "SELECT * FROM books WHERE id LIKE @FilterByTextValue ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            FilterByTextValue = "b"
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(1);

        var parameter = query.Parameters.First();

        parameter.Key.Should().Be("@FilterByTextValue");
        parameter.Value.Should().Be("%b%");
    }

    [Theory]
    [InlineData(nameof(Book.Author), nameof(BookSqlite.Author), ReadBooksRequest.FieldType.Text, "testAuthor")]
    [InlineData(nameof(Book.Title), nameof(BookSqlite.Title), ReadBooksRequest.FieldType.Text, "testTitle")]
    [InlineData(nameof(Book.Description), nameof(BookSqlite.Description), ReadBooksRequest.FieldType.Text, "test description phrase")]
    [InlineData(nameof(Book.Genre), nameof(BookSqlite.Genre), ReadBooksRequest.FieldType.Text, "TestGenre")]
    public void Read_ReturnsSelectAllBooksQuery_WhereLike_ValueToFilterBy_WhenSortBy_TextValue(string fieldToSortBy, string sqliteFieldToSortBy, ReadBooksRequest.FieldType fieldType, string valueToFilterBy)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} LIKE @FilterByTextValue ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = fieldToSortBy,
            FilterByTextValue = valueToFilterBy,
            SortResultByFieldType = fieldType
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        var parameter = query.Parameters.First();

        parameter.Key.Should().Be("@FilterByTextValue");
        parameter.Value.Should().Be($"%{valueToFilterBy}%");
    }

    [Theory]
    [InlineData(nameof(Book.Price), nameof(BookSqlite.Price), ReadBooksRequest.FieldType.Numeric, 5)]
    [InlineData(nameof(Book.Price), nameof(BookSqlite.Price), ReadBooksRequest.FieldType.Numeric, 10.00)]
    public void Read_ReturnsSelectAllBooksQuery_Where_FilterByDoubleValue(string fieldToSortBy, string sqliteFieldToSortBy, ReadBooksRequest.FieldType fieldType, double valueToFilterBy)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} = @FilterByDoubleValue ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = fieldToSortBy,
            FilterByDoubleValue = valueToFilterBy,
            SortResultByFieldType = fieldType
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        var parameter = query.Parameters.First();

        parameter.Key.Should().Be("@FilterByDoubleValue");
        parameter.Value.Should().Be(valueToFilterBy);
    }

    [Theory]
    [InlineData(nameof(Book.Price), nameof(BookSqlite.Price), ReadBooksRequest.FieldType.Numeric, 5, 99.05)]
    [InlineData(nameof(Book.Price), nameof(BookSqlite.Price), ReadBooksRequest.FieldType.Numeric, 10.00, 19.33)]
    public void Read_ReturnsSelectAllBooksQuery_Where_FilterByDoubleValue_Ranged(string fieldToSortBy, string sqliteFieldToSortBy, ReadBooksRequest.FieldType fieldType, double valueToFilterBy, double valueToFilterBy2)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} BETWEEN @FilterByDoubleValue AND @FilterByDoubleValue2 ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT 20;";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = fieldToSortBy,
            FilterByDoubleValue = valueToFilterBy,
            FilterByDoubleValue2 = valueToFilterBy2,
            SortResultByFieldType = fieldType
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        var parameter = query.Parameters.First();
        parameter.Key.Should().Be("@FilterByDoubleValue");
        parameter.Value.Should().Be(valueToFilterBy);

        var parameter2 = query.Parameters.Last();
        parameter2.Key.Should().Be("@FilterByDoubleValue2");
        parameter2.Value.Should().Be(valueToFilterBy2);
    }

    [Theory]
    [InlineData(nameof(Book.PublishDate), nameof(BookSqlite.Publish_date), ReadBooksRequest.DatePrecision.Day, 2015, 8, 15)]
    [InlineData(nameof(Book.PublishDate), nameof(BookSqlite.Publish_date), ReadBooksRequest.DatePrecision.Month, 2015, 8, 15)]
    [InlineData(nameof(Book.PublishDate), nameof(BookSqlite.Publish_date), ReadBooksRequest.DatePrecision.Year, 2015, 8, 15)]
    public void Read_ReturnsSelectAllBooksQuery_Where_FilterByDateValue(string fieldToSortBy, string sqliteFieldToSortBy, ReadBooksRequest.DatePrecision datePrecision, int year, int month, int day)
    {
        // Arrange
        var datePrecisionLength = datePrecision switch
        {
            ReadBooksRequest.DatePrecision.None => 0,
            ReadBooksRequest.DatePrecision.Year => 4,
            ReadBooksRequest.DatePrecision.Month => 7,
            ReadBooksRequest.DatePrecision.Day => 10,
            _ => throw new NotImplementedException($"Date precision {datePrecision.ToString()} not implemented")
        };

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = fieldToSortBy,
            SortResultByFieldType = ReadBooksRequest.FieldType.Date,
            FilterByDateValue = new DateOnly(year, month, day),
            FilterByDatePrecision = datePrecision
        };

        var dateOnlyString = readBooksRequest.FilterByDateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var expectedQuery = $"SELECT * FROM books WHERE substring({sqliteFieldToSortBy.ToLower()},1,{datePrecisionLength}) = substring('{dateOnlyString}',1,{datePrecisionLength}) ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT 20;";

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 15)]
    [InlineData(1, 2)]
    [InlineData(1, 1)]
    public void CreatePaginationQuery_Returns_Correct_SQL_for_Id_WhenFirstPage(int pageNumber, int pageSize)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT {pageSize};";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = true,
            BookParameters = new Shared.RequestParameters.BookParameters { PageNumber = pageNumber, PageSize = pageSize }
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    [Theory]
    [InlineData(2, 10, 10)]
    [InlineData(5, 10, 40)]
    [InlineData(2, 5, 5)]
    [InlineData(3, 5, 10)]
    [InlineData(2, 50, 50)]
    public void CreatePaginationQuery_Returns_Correct_SQL_for_Id_WhenNotFirstPage(int pageNumber, int pageSize, int expectedToSkip)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books WHERE id NOT IN ( SELECT id FROM books ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT {expectedToSkip}) ORDER BY CAST(SUBSTRING(id,2,9) AS NUMERIC) LIMIT {pageSize};";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = true,
            BookParameters = new Shared.RequestParameters.BookParameters { PageNumber = pageNumber, PageSize = pageSize }
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    [Theory]
    [InlineData(1, 10, nameof(Book.Author), nameof(BookSqlite.Author))]
    [InlineData(1, 10, nameof(Book.Description), nameof(BookSqlite.Description))]
    [InlineData(1, 5, nameof(Book.Genre), nameof(BookSqlite.Genre))]
    [InlineData(1, 50, nameof(Book.Price), nameof(BookSqlite.Price))]
    [InlineData(1, 10, nameof(Book.PublishDate), nameof(BookSqlite.Publish_date))]
    [InlineData(1, 5, nameof(Book.Title), nameof(BookSqlite.Title))]
    public void CreatePaginationQuery_Returns_Correct_SQL_for_not_ID_FirstPage(int pageNumber, int pageSize, string fieldToSortBy, string sqliteFieldToSortBy)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT {pageSize};";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = true,
            SortResultByField = fieldToSortBy,
            BookParameters = new Shared.RequestParameters.BookParameters { PageNumber = pageNumber, PageSize = pageSize }
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    [Theory]
    [InlineData(2, 10, 10, nameof(Book.Author), nameof(BookSqlite.Author))]
    [InlineData(3, 10, 20, nameof(Book.Description), nameof(BookSqlite.Description))]
    [InlineData(4, 5, 15, nameof(Book.Genre), nameof(BookSqlite.Genre))]
    [InlineData(2, 50, 50, nameof(Book.Price), nameof(BookSqlite.Price))]
    [InlineData(5, 10, 40, nameof(Book.PublishDate), nameof(BookSqlite.Publish_date))]
    [InlineData(3, 5, 10, nameof(Book.Title), nameof(BookSqlite.Title))]
    public void CreatePaginationQuery_Returns_Correct_SQL_for_not_ID_NotFirstPage(int pageNumber, int pageSize, int expectedToSkip, string fieldToSortBy, string sqliteFieldToSortBy)
    {
        // Arrange
        var expectedQuery = $"SELECT * FROM books WHERE id NOT IN ( SELECT id FROM books ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT {expectedToSkip}) ORDER BY {sqliteFieldToSortBy.ToLower()} LIMIT {pageSize};";

        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = true,
            SortResultByField = fieldToSortBy,
            BookParameters = new Shared.RequestParameters.BookParameters { PageNumber = pageNumber, PageSize = pageSize }
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }

    // --------------------- UPDATE ------------------------------------

    [Fact]
    public void Update_ThrowsException_When_Book_Null()
    {
        _queryCreator.Invoking(y => y.Update(null))
            .Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'bookSqlite cannot be null.')");
    }

    [Fact]
    public void Update_ThrowsException_When_BookId_Null()
    {
        _queryCreator.Invoking(y => y.Update(new BookSqlite()))
            .Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'Id cannot be null.')");
    }

    [Fact]
    public void Update_ReturnsUpdateBookQuery_AndIgnoresNullValues_AndIgnoresId()
    {
        // Arrange
        var updateToThisBook = new BookSqlite()
        {
            Author = "Updated Author",
            Title = "New cooler title",
            Description = null,
            Id = "B16",
            Genre = null,
            Price = 10,
            Publish_date = "1999-12-31"
        };

        var expectedQuery = "UPDATE books SET author = @Author, title = @Title, price = @Price, publish_date = @Publish_date WHERE id = @Id;";

        // Act
        var sqlQuery = _queryCreator.Update(updateToThisBook);

        // Assert
        sqlQuery.QueryString.ToString().Should().Be(expectedQuery);

        var parameters = sqlQuery.Parameters;

        parameters.Count.Should().Be(5);

        parameters["@Id"].Should().Be("B16");
        parameters["@Author"].Should().Be(updateToThisBook.Author);
        parameters["@Title"].Should().Be(updateToThisBook.Title);
        parameters["@Price"].Should().Be(updateToThisBook.Price);
        parameters["@Publish_date"].Should().Be("1999-12-31");
    }

    [Fact]
    public void Update_ReturnsUpdateBookQuery_AndIgnoresNullValues_PriceAndDate()
    {
        // Arrange
        var bookId = "B16";

        var updateToThisBook = new BookSqlite()
        {
            Author = null,
            Title = "Only change this one",
            Description = null,
            Id = bookId,
            Genre = null,
            Price = double.MinValue,
            Publish_date = string.Empty
        };

        var expectedQuery = "UPDATE books SET title = @Title WHERE id = @Id;";

        // Act
        var sqlQuery = _queryCreator.Update(updateToThisBook);

        // Assert
        sqlQuery.QueryString.ToString().Should().Be(expectedQuery);

        var parameters = sqlQuery.Parameters;

        parameters.Count.Should().Be(2);

        parameters["@Id"].Should().Be(bookId);
        parameters["@Title"].Should().Be(updateToThisBook.Title);
    }

    // --------------------- DELETE ------------------------------------

    [Fact]
    public void Delete_ThrowsException_When_BookId_Null()
    {
        _queryCreator.Invoking(y => y.Delete(null))
            .Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'bookId cannot be null.')");
    }

    [Fact]
    public void Delete_ReturnsDeleteBookQuery()
    {
        // Arrange
        var bookId = "B16";

        var expectedQuery = "DELETE FROM books WHERE id=@Id;";

        // Act
        var sqlQuery = _queryCreator.Delete(bookId);

        // Assert
        sqlQuery.QueryString.ToString().Should().Be(expectedQuery);

        var parameters = sqlQuery.Parameters;

        parameters.Count.Should().Be(1);

        parameters["@Id"].Should().Be(bookId);
    }

    // --------------------- GET VALUE ------------------------------------

    [Fact]
    public void GetValueQuery_ReturnsSelectMaxQuery_WithCast()
    {
        // Arrange
        var expectedQuery = "SELECT id FROM books WHERE CAST(SUBSTRING(id, 2) AS UNSIGNED) = (SELECT MAX(CAST(SUBSTRING(id, 2) AS UNSIGNED)) FROM books);";

        var getValuesRequest = new GetValueRequest
        {
            ColumnName = "Id",
            IgnoreFirstCharacters = 1,
            GetMaxValue = true
        };

        // Act
        var query = _queryCreator.GetValueQuery(getValuesRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(0);
    }
}
