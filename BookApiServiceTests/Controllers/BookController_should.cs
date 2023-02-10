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
}
