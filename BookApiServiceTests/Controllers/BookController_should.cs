using BookApi;
using BookApiServiceTests.TestData;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace BookApiServiceTests.Controllers;

public class BookController_should : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    private const string ControllerBaseRoute = "api/books";

    public BookController_should(WebApplicationFactory<Startup> factory)
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
        var responseStart = await _client.GetAsync($"{ControllerBaseRoute}");

        // Assert
        responseStart.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await responseStart.Content.ReadAsStringAsync();

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
        var responseStart = await _client.GetAsync($"{ControllerBaseRoute}/{field}");

        // Assert
        responseStart.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await responseStart.Content.ReadAsStringAsync();

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
        var responseStart = await _client.GetAsync($"{ControllerBaseRoute}/{field}/{filterValue}");

        // Assert
        responseStart.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await responseStart.Content.ReadAsStringAsync();

        var booksCollection = JsonSerializer.Deserialize<List<TestBook>>(content);

        //Asserting only by ID first to assert sorting and to improve readability of test failure message (hard to compare sorting order otherwise)
        booksCollection.Select(b => b.Id).Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult.Select(e => e.Id), config => config.WithStrictOrdering());

        booksCollection.Should().NotBeEmpty()
            .And.BeEquivalentTo(expectedResult);
    }
}
