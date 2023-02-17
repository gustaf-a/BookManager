using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared;

namespace BookApi.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    private IBookService _bookService => _serviceManager.BookService;

    public BookController(IServiceManagerFactory serviceManagerFactory)
    {
        _serviceManager = serviceManagerFactory.GetService();
    }

    /// <summary>
    /// Creates a book from JSON in request body with a generated ID.
    /// Example payload: 
    /// { 
    ///     "author": "TestLastname, TestFirstName", 
    ///     "title": "Test Book", 
    ///     "genre": "Test genre", 
    ///     "price": 38.95,
    ///     "publish_date": "2008-06-01",
    ///     "description": "Test description" 
    /// }
    /// </summary>
    /// <returns>Returns created book</returns>
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
    {
        if (bookDto is null)
            return BadRequest();

        var result = await _bookService.CreateBook(bookDto);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Updates a book from JSON in request body with a generated ID.
    /// Example payload: 
    /// { 
    ///     "author": "TestLastname, TestFirstName", 
    ///     "title": "Test Book", 
    ///     "genre": "Test genre", 
    ///     "price": 38.95,
    ///     "publish_date": "2008-06-01",
    ///     "description": "Test description" 
    /// }
    /// </summary>
    /// <returns>Returns updated book</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(string id, [FromBody] BookDto bookDto)
    {
        if (bookDto is null)
            return BadRequest();

        await _bookService.UpdateBook(bookDto, id);

        return NoContent();
    }

    /// <summary>
    /// Deletes a book by ID.
    /// </summary>
    /// <returns>Returns null object if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(string id)
    {
        var result = await _bookService.DeleteBook(id);

        if (!result)
            return new StatusCodeResult(500);

        return NoContent();
    }

    /// <summary>
    /// Returns all books as an unsorted JSON collection.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = false
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by ID.
    /// </summary>
    /// <param name="filterValue">Optional text value that ID of the books must contain.</param>
    [HttpGet("id/{filterValue?}")]
    public async Task<IActionResult> GetAllBooks_ById(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Id),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by author.
    /// </summary>
    /// <param name="filterValue">Optional text value that author of the books must contain.</param>
    [HttpGet("author/{filterValue?}")]
    public async Task<IActionResult> GetAllBooks_ByAuthor(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Author),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by title.
    /// </summary>
    /// <param name="filterValue">Optional text value that title of the books must contain.</param>
    [HttpGet("title/{filterValue?}")]
    public async Task<IActionResult> GetAllBooks_ByTitle(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Title),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by genre.
    /// </summary>
    /// <param name="filterValue">Optional text value that genre of the books must contain.</param>
    [HttpGet("genre/{filterValue?}")]
    public async Task<IActionResult> GetAllBooks_ByGenre(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Genre),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books with a certain price as a JSON collection.
    /// </summary>
    /// <param name="filterValue">Optional text value that genre of the books must contain.</param>
    [HttpGet("price/{filterValue?}")]
    public async Task<IActionResult> GetAllBooks_ByPrice(double filterValue = double.MinValue)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDoubleValue = filterValue,
            SortResultByFieldType = ReadBooksRequest.FieldType.Numeric,
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books between two double values as a JSON collection sorted by price.
    /// Example: "price/5.95&10"
    /// </summary>
    /// <param name="lowerPrice">Lower value when searching a range of prices.</param>
    /// <param name="higherPrice">Higher value when searching a range of prices.</param>
    [HttpGet("price/{lowerPrice}&{higherPrice}")]
    public async Task<IActionResult> GetAllBooks_ByPrice(double lowerPrice, double higherPrice)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDoubleValue = lowerPrice < higherPrice ? lowerPrice : higherPrice,
            FilterByDoubleValue2 = lowerPrice < higherPrice ? higherPrice : lowerPrice,
            SortResultByFieldType = ReadBooksRequest.FieldType.Numeric
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by published date.
    /// Use parameters to filter specific dates.
    /// Example: "published/2012", "published/2012/8" or "published/2012/8/15"
    /// </summary>
    /// <param name="year">If provided only shows books from this year.</param>
    /// <param name="month">If provided with year only shows books from this month.</param>
    /// <param name="day">If provided with year and month only shows books from this date.</param>
    [HttpGet("published/{year:int?}/{month:int?}/{day:int?}")]
    public async Task<IActionResult> GetAllBooks_ByPublishDate(int year = int.MinValue, int month = int.MinValue, int day = int.MinValue)
    {
        var filterByDatePrecision = ReadBooksRequest.FindDatePrecision(year, month, day);

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.PublishDate),
            SortResultByFieldType = ReadBooksRequest.FieldType.Date,
            FilterByDateValue = filterByDatePrecision.GetDateOnly(year, month, day),
            FilterByDatePrecision = filterByDatePrecision
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }

    /// <summary>
    /// Returns books as a JSON collection sorted by description.
    /// </summary>
    /// <param name="filterValue">Optional text value that description of the books must contain.</param>
    [HttpGet("description/{filterValue?}")]
    public async Task<IActionResult> GetAllBooks_ByDescription(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Description),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        var result = await _bookService.ReadBooks(readBooksRequest);

        return Ok(result);
    }
}
