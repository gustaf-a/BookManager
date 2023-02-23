using BookApi;
using BookApiServiceTests.TestData;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace BookApiServiceTests.Controllers;

/// <summary>
/// Service tests for BookController testing Read function.
/// </summary>
public class BookController_should_getBooks : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    private const string ControllerBaseRoute = "api/books";

    public BookController_should_getBooks(WebApplicationFactory<Startup> factory)
    {
        _client = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        //Add services to override with fake/mock services here
                    });
                }
                )
                .CreateClient();
    }

    [Fact]
    public async Task GetAllBooks_WhenBaseRouteCalled()
    {
        // Arrange
        var expectedResult = TestDataHelper.GetBooks();

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        booksCollection.Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("B1 B2 B3 B4 B5 B6 B7 B8 B9 B10 B11 B12 B13", "id")]
    [InlineData("B13 B3 B4 B8 B9 B1 B10 B2 B6 B12 B11 B7 B5", "author")]
    [InlineData("B7 B2 B11 B9 B3 B8 B1 B5 B4 B10 B13 B12 B6", "description")]
    [InlineData("B1 B10 B11 B12 B13 B2 B3 B4 B5 B8 B6 B7 B9", "genre")]
    [InlineData("B6 B7 B8 B2 B3 B4 B9 B5 B13 B1 B11 B10 B12", "price")]
    [InlineData("B5 B6 B7 B9 B3 B8 B10 B2 B4 B11 B12 B13 B1", "published")]
    [InlineData("B13 B8 B1 B11 B6 B3 B12 B10 B2 B4 B9 B7 B5", "title")]
    public async Task GetAllBooks_SortedByField_WhenFieldIsAddedToRoute(string bookIdsToGet, string field)
    {
        // Arrange
        var expectedResult = TestDataHelper.GetBooks(bookIdsToGet);

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}/{field}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        //Asserting only by ID first to assert sorting and to improve readability of test failure message (hard to compare sorting order otherwise)
        booksCollection.Select(b => b.Id).Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult.Select(e => e.Id), config => config.WithStrictOrdering());

        booksCollection.Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("B1 B2 B3 B4 B5 B6 B7 B8 B9 B10 B11 B12 B13", "id", "b")]
    [InlineData("B1 B10 B11 B12 B13", "id", "b1")]
    [InlineData("B1", "author", "joe")]
    [InlineData("B1", "author", "kut")]
    [InlineData("B1", "title", "deploy")]
    [InlineData("B1", "title", "jruby")]
    [InlineData("B1 B10 B11 B12 B13", "genre", "com")]
    [InlineData("B1 B10 B11 B12 B13", "genre", "ter")]
    [InlineData("B1 B13", "description", "deploy")]
    [InlineData("B1", "description", "applications")]
    public async Task GetBooks_FilteredBy_TextValues_SortedByField_WhenFieldIsAddedToRoute(string bookIdsToGet, string field, string filterValue)
    {
        // Arrange
        var expectedResult = TestDataHelper.GetBooks(bookIdsToGet);

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}/{field}/{filterValue}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        //Asserting only by ID first to assert sorting and to improve readability of test failure message (hard to compare sorting order otherwise)
        booksCollection.Select(b => b.Id).Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult.Select(e => e.Id), config => config.WithStrictOrdering());

        booksCollection.Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("B1", "price", "33.0")]
    [InlineData("B2 B3 B4", "price", "5.95")]
    [InlineData("B1", "price", "33")]
    [InlineData("B1", "price", "0033,")]
    [InlineData("B1", "price", "33&33.05")]
    [InlineData("B1", "price", "00033&33.05")]
    [InlineData("B2 B3 B4", "price", "5.95&5.95")]
    [InlineData("B6 B7 B8 B2 B3 B4", "price", "4&6")]
    [InlineData("B6 B7 B8 B2 B3 B4", "price", "5.95&4.95")]
    public async Task GetBooks_FilteredBy_DoubleValues_SortedByField_WhenFieldIsAddedToRoute(string bookIdsToGet, string field, string filterValue)
    {
        // Arrange
        var expectedResult = TestDataHelper.GetBooks(bookIdsToGet);

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}/{field}/{filterValue}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        //Asserting only by ID first to assert sorting and to improve readability of test failure message (hard to compare sorting order otherwise)
        booksCollection.Select(b => b.Id).Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult.Select(e => e.Id), config => config.WithStrictOrdering());

        booksCollection.Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("price", "G5.95")]
    [InlineData("price", "33.0%33.05")]
    [InlineData("price", "5.95&")]
    [InlineData("price", "&5")]
    [InlineData("price", "5.95&'5'")]
    public async Task GetBooks_FilteredBy_DoubleValues_Returns_BadRequest_WhenNotDouble(string field, string filterValue)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}/{field}/{filterValue}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("B13 B1", "published", "2012")]
    [InlineData("B1", "published", "2012/8")]
    [InlineData("B1", "published", "2012/8/15")]
    public async Task GetBooks_FilteredBy_DateValues_SortedByField_WhenFieldIsAddedToRoute(string bookIdsToGet, string field, string filterValue)
    {
        // Arrange
        var expectedResult = TestDataHelper.GetBooks(bookIdsToGet);

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}/{field}/{filterValue}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        //Asserting only by ID first to assert sorting and to improve readability of test failure message (hard to compare sorting order otherwise)
        booksCollection.Select(b => b.Id).Should().BeEquivalentTo(expectedResult.Select(e => e.Id), config => config.WithStrictOrdering());

        booksCollection.Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("B1 B2 B3 B4 B5 B6 B7 B8 B9 B10 B11 B12 B13", "id", "13", "1")]
    [InlineData(TestDataHelper.EmptyBookCollectionId, "id", "20", "2")]
    [InlineData("B1 B2 B3 B4 B5 B6 B7 B8 B9 B10", "id", "10", "1")]
    [InlineData("B11 B12 B13", "id", "10", "2")]
    [InlineData("B13 B3 B4 B8 B9 B1 B10 B2 B6 B12 B11 B7 B5", "author", "13", "1")]
    [InlineData("B13 B3", "author", "2", "1")]
    [InlineData("B4 B8", "author", "2", "2")]
    [InlineData("B7 B2 B11 B9 B3 B8 B1 B5 B4 B10 B13 B12 B6", "description", "13", "1")]
    [InlineData("B8", "description", "1", "6")]
    [InlineData("B4 B5 B8 B6 B7 B9", "genre", "7", "2")]
    [InlineData(TestDataHelper.EmptyBookCollectionId, "genre", "13", "2")]
    [InlineData("B12 B13 B1", "published", "5", "3")]
    [InlineData("B12 B10", "title", "2", "4")]
    public async Task GetAllBooks_SortedByField_WhenFieldIsAddedToRoute_WithCorrectPaging(string bookIdsToGet, string field, string pageSize, string pageNumber)
    {
        // Arrange
        var expectedResult = TestDataHelper.GetBooks(bookIdsToGet);

        // Act
        var response = await _client.GetAsync($"{ControllerBaseRoute}/{field}?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        //Asserting only by ID first to assert sorting and to improve readability of test failure message (hard to compare sorting order otherwise)
        booksCollection.Select(b => b.Id).Should().BeEquivalentTo(expectedResult.Select(e => e.Id), config => config.WithStrictOrdering());

        booksCollection.Should().BeEquivalentTo(expectedResult);
    }
}
