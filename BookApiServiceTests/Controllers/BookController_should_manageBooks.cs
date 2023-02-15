using BookApi;
using BookApiServiceTests.TestData;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
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
        var payloadCreate = new
        {
            author = expectedBook.Author,
            title = expectedBook.Title,
            genre = expectedBook.Genre,
            price = expectedBook.Price,
            publish_date = "2008-06-01", // As string directly to improve test readability
            description = expectedBook.Description
        };
        
        var createdBookResponse = await _client.PostAsync($"{ControllerBaseRoute}", GetJsonStringContent(payloadCreate));

        //// Assert

        createdBookResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createContent = await createdBookResponse.Content.ReadAsStringAsync();

        var createdBook = System.Text.Json.JsonSerializer.Deserialize<TestBook>(createContent);

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
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);


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

    [Fact]
    public async Task CreateBook_Updates_Reads_Deletes()
    {
        // -------------- Create book --------------

        // Arrange

        var expectedBook = new TestBook
        {
            Author = "TestLastname, TestFirstName",
            Title = "Test Book",
            Genre = "Test genre",
            Price = 38.95,
            PublishDate = new DateOnly(2008, 6, 1),
            Description = "Test description"
        };

        var payloadCreate = new
        {
            author = expectedBook.Author,
            title = expectedBook.Title,
            genre = expectedBook.Genre,
            price = expectedBook.Price,
            publish_date = "2008-06-01", // As string directly to improve test readability
            description = expectedBook.Description
        };

        // Act
        var createdBookResponse = await _client.PostAsync($"{ControllerBaseRoute}", GetJsonStringContent(payloadCreate));

        // Assert
        createdBookResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createContent = await createdBookResponse.Content.ReadAsStringAsync();

        var createdBook = JsonSerializer.Deserialize<TestBook>(createContent);

        createdBook.Should().NotBeNull()
            .And.BeEquivalentTo(expectedBook, options =>
                options.Excluding(createdTestBook => createdTestBook.Id));

        // -------------- Update only description of book --------------

        var expectedPartiallyUpdatedBook = new TestBook
        {
            Id = createdBook.Id,
            Author = "TestLastname, TestFirstName",
            Title = "Test Book",
            Genre = "Test genre",
            Price = 38.95,
            PublishDate = new DateOnly(2008, 6, 1),
            Description = "Test very much updated description"
        };

        var payloadPartialUpdate = new
        {
            description = expectedPartiallyUpdatedBook.Description
        };

        // Act
        var partialupdateResponse = await _client.PutAsync($"{ControllerBaseRoute}/{createdBook.Id}", GetJsonStringContent(payloadPartialUpdate));

        // Assert
        partialupdateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var partialUpdateContent = await partialupdateResponse.Content.ReadAsStringAsync();

        var partiallyUpdatedBook = JsonSerializer.Deserialize<TestBook>(partialUpdateContent);

        partiallyUpdatedBook.Should().NotBeNull()
            .And.BeEquivalentTo(expectedPartiallyUpdatedBook);

        // -------------- Update book --------------

        var expectedUpdatedBook = new TestBook
        {
            Id = createdBook.Id,
            Author = "TestUpdatedLastname, TestUpdatedFirstName",
            Title = "Updated Book",
            Genre = "Updated genre",
            Price = 10,
            PublishDate = new DateOnly(2015, 4, 4),
            Description = "Test updated description"
        };

        var payloadUpdate = new
        {
            author = expectedUpdatedBook.Author,
            title = expectedUpdatedBook.Title,
            genre = expectedUpdatedBook.Genre,
            price = expectedUpdatedBook.Price,
            publish_date = "2015-04-04", // As string directly to improve test readability
            description = expectedUpdatedBook.Description
        };

        // Act
        var updateResponse = await _client.PutAsync($"{ControllerBaseRoute}/{createdBook.Id}", GetJsonStringContent(payloadUpdate));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateContent = await updateResponse.Content.ReadAsStringAsync();

        var updatedBook = JsonSerializer.Deserialize<TestBook>(updateContent);

        updatedBook.Should().NotBeNull()
            .And.BeEquivalentTo(expectedUpdatedBook);

        // -------------- Get book --------------

        // Act
        var getResponse = await _client.GetAsync($"{ControllerBaseRoute}/id/{createdBook.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getContent = await getResponse.Content.ReadAsStringAsync();

        var getByIdBook = JsonSerializer.Deserialize<List<TestBook>>(getContent).FirstOrDefault();

        getByIdBook.Should().NotBeNull()
            .And.BeEquivalentTo(updatedBook);

        // -------------- Delete book --------------

        // Act
        var deleteResponse = await _client.DeleteAsync($"{ControllerBaseRoute}/{getByIdBook.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static HttpContent GetJsonStringContent(object payload)
        => new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
}
