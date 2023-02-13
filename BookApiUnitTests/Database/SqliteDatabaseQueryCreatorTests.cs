using BookApi.Data;
using BookApi.Database;
using BookApi.Database.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Text;

namespace BookApiUnitTests.Database;

public class SqliteDatabaseQueryCreatorTests
{
    private readonly IDatabaseQueryCreator _queryCreator;

    private const string AppSettingsJson =
@"{
    ""Database"": {
        ""BooksTableName"":  ""books"",
        ""IdNumberMaxLength"": 9,
        ""IdCharacterPrefixLength"": 2
    }
}";

    public SqliteDatabaseQueryCreatorTests()
    {
        //It's not possible to mock the IConfiguration as Get<>() is an extension method, so this is a working method
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(AppSettingsJson)));

        _queryCreator = new SqliteDatabaseQueryCreator(builder.Build());
    }

    // --------------------- CREATE ------------------------------------

    //[Fact]
    public void Create_ReturnsCreateBookQuery()
    {
        // Arrange
        var expectedQuery = "nope";

        var book = new Book
        {
            // Id Should be auto created
            Author = "Testson, Tester",
            Title = "How to test",
            Genre = "Testing",
            Price = 4.99,
            Description = "The most testing book you'll ever read.",
            PublishDate = new DateOnly(2023, 02, 11)
        };

        // Act
        var query = _queryCreator.Create(book);

        // Assert
        query.Should().Be(expectedQuery);
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
        var expectedQuery = "SELECT * FROM books;";

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
        //3 is for starting point when IdCharacterPrefixLength is set to 2 in appsettings
        var expectedQuery = "SELECT * FROM books ORDER BY CAST(SUBSTRING(id,3,9) AS NUMERIC);";

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
        var expectedQuery = $"SELECT * FROM books ORDER BY {sqliteFieldToSortBy.ToLower()} ASC;";

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
        var expectedQuery = "SELECT * FROM books WHERE id LIKE @FilterByTextValue ORDER BY CAST(SUBSTRING(id,3,9) AS NUMERIC);";

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
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} LIKE @FilterByTextValue ORDER BY {sqliteFieldToSortBy.ToLower()} ASC;";

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
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} = @FilterByDoubleValue ORDER BY {sqliteFieldToSortBy.ToLower()} ASC;";

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
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} BETWEEN @FilterByDoubleValue AND @FilterByDoubleValue2 ORDER BY {sqliteFieldToSortBy.ToLower()} ASC;";

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
            ReadBooksRequest.DatePrecision.Day => 10
        };

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = fieldToSortBy,
            SortResultByFieldType = ReadBooksRequest.FieldType.Date,
            FilterByDateValue = new DateOnly(year, month, day),
            FilterByDatePrecision = datePrecision
        };

        var dateOnlyString = readBooksRequest.FilterByDateValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var expectedQuery = $"SELECT * FROM books WHERE substring({sqliteFieldToSortBy.ToLower()},1,{datePrecisionLength}) = substring('{dateOnlyString}',1,{datePrecisionLength}) ORDER BY {sqliteFieldToSortBy.ToLower()} ASC;";

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);
    }

    // --------------------- UPDATE ------------------------------------

    //[Fact]
    public void Update_ReturnsUpdateBookQuery()
    {
        // Arrange
        var expectedQuery = "nope";

        var book = new Book
        {
            // Id Should be auto created
            Author = "Testson, Tester",
            Title = "How to test",
            Genre = "Testing",
            Price = 4.99,
            Description = "The most testing book you'll ever read.",
            PublishDate = new DateOnly(2023, 02, 11)
        };

        // Act
        var query = _queryCreator.Update(book);

        // Assert
        query.Should().Be(expectedQuery);
    }

    // --------------------- DELETE ------------------------------------

    //[Fact]
    public void Delete_ReturnsDeleteBookQuery()
    {
        // Arrange
        var expectedQuery = "nope";

        var book = new Book
        {
            // Id Should be auto created
            Author = "Testson, Tester",
            Title = "How to test",
            Genre = "Testing",
            Price = 4.99,
            Description = "The most testing book you'll ever read.",
            PublishDate = new DateOnly(2023, 02, 11)
        };

        // Act
        var query = _queryCreator.Delete(book);

        // Assert
        query.Should().Be(expectedQuery);
    }
}
