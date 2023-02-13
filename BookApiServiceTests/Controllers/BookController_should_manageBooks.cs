using BookApi;
using BookApiServiceTests.TestData;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BookApiServiceTests.Controllers;

/// <summary>
/// Service tests for BookController testing Create, Update and Delete function.
/// </summary>
public class BookController_should_manageBooks : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    private const string ControllerBaseRoute = "api/books";

    public BookController_should_manageBooks(WebApplicationFactory<Startup> factory)
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
    public async Task CreateBook_Read_Deletes_AndTryRead()
    {
        // -------------- Create book --------------

        var expectedBook = new TestBook
        {
            Author = "TestLastname, TestFirstName",
            Title = "Test Book",
            Genre = "Test genre",
            Price = 38.95,
            PublishDate = new DateOnly(2008, 6, 1),
            Description = "Test description"
        };

        // Arrange
        var payload = new
        {
            author = expectedBook.Author,
            title = expectedBook.Title,
            genre = expectedBook.Genre,
            price = expectedBook.Price,
            publish_date = "2008-06-01", // As string directly to improve test readability
            description = expectedBook.Description
        };

        // Act
        var createdBookResponse = await _client.PostAsync($"{ControllerBaseRoute}", new StringContent(JsonSerializer.Serialize(payload)));

        // Assert
        createdBookResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await createdBookResponse.Content.ReadAsStringAsync();

        var createdBook = JsonSerializer.Deserialize<TestBook>(content);

        createdBook.Should().NotBeNull()
            .And.BeEquivalentTo(expectedBook, options =>
                options.Excluding(createdTestBook => createdTestBook.Id));


        // -------------- Get book --------------

        // Act
        var getResponse = await _client.GetAsync($"{ControllerBaseRoute}/id/{createdBook.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getContent = await getResponse.Content.ReadAsStringAsync();

        var getByIdBook = JsonSerializer.Deserialize<List<TestBook>>(getContent).FirstOrDefault();

        getByIdBook.Should().NotBeNull()
            .And.BeEquivalentTo(createdBook);


        // -------------- Delete book --------------

        // Act
        var deleteResponse = await _client.DeleteAsync($"{ControllerBaseRoute}/{createdBook.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);


        // -------------- Attempt to get deleted book --------------

        // Act
        var getDeletedBookResponse = await _client.GetAsync($"{ControllerBaseRoute}/id/{createdBook.Id}");

        // Assert
        getDeletedBookResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getDeletedBookContent = await getDeletedBookResponse.Content.ReadAsStringAsync();

        var deletedBook = JsonSerializer.Deserialize<List<TestBook>>(getDeletedBookContent);

        deletedBook.Should().NotBeNull()
            .And.BeEquivalentTo(new List<TestBook>());
    }
}
