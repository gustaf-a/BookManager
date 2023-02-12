using BookApi.Data;
using BookApi.Database;
using BookApi.Database.SQLite;
using Microsoft.Extensions.Configuration;
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
            FieldToSortBy = nameof(Book.Id),
            Type = ReadBooksRequest.FieldType.Text
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
            FieldToSortBy = fieldToSortBy,
            Type = fieldType
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
        var expectedQuery = "SELECT * FROM books WHERE id LIKE @ValueToFilterBy ORDER BY CAST(SUBSTRING(id,3,9) AS NUMERIC);";

        var readBooksRequest = new ReadBooksRequest
        {
            FilterByValue = true,
            ValueToFilterBy = "b"
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        query.Parameters.Count().Should().Be(1);

        var parameter = query.Parameters.First();

        parameter.Key.Should().Be("@ValueToFilterBy");
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
        var expectedQuery = $"SELECT * FROM books WHERE {sqliteFieldToSortBy.ToLower()} LIKE @ValueToFilterBy ORDER BY {sqliteFieldToSortBy.ToLower()} ASC;";

        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy = fieldToSortBy,
            FilterByValue = true,
            ValueToFilterBy = valueToFilterBy,
            Type = fieldType
        };

        // Act
        var query = _queryCreator.Read(readBooksRequest);

        // Assert
        query.QueryString.ToString().Should().Be(expectedQuery);

        var parameter = query.Parameters.First();

        parameter.Key.Should().Be("@ValueToFilterBy");
        parameter.Value.Should().Be($"%{valueToFilterBy}%");
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
