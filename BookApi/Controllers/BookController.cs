using BookApi.Data;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;

[ApiController]
[Produces("application/json")] // Ensures JSON is returned
[Route("api/books")]
public class BookController : Controller
{
    private IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Returns all books as an unsorted JSON collection.
    /// </summary>
    [HttpGet]
    public JsonResult GetAllBooks()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResult = false
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by ID.
    /// </summary>
    /// <param name="filterValue">Optional text value that ID of the books must contain.</param>
    [HttpGet("id/{filterValue?}")]
    public JsonResult GetAllBooks_ById(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Id),
            FilterByText = ShouldFilterByText(filterValue),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by author.
    /// </summary>
    /// <param name="filterValue">Optional text value that author of the books must contain.</param>
    [HttpGet("author/{filterValue?}")]
    public JsonResult GetAllBooks_ByAuthor(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Author),
            FilterByText = ShouldFilterByText(filterValue),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by title.
    /// </summary>
    /// <param name="filterValue">Optional text value that title of the books must contain.</param>
    [HttpGet("title/{filterValue?}")]
    public JsonResult GetAllBooks_ByTitle(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Title),
            FilterByText = ShouldFilterByText(filterValue),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by genre.
    /// </summary>
    /// <param name="filterValue">Optional text value that genre of the books must contain.</param>
    [HttpGet("genre/{filterValue?}")]
    public JsonResult GetAllBooks_ByGenre(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Genre),
            FilterByText = ShouldFilterByText(filterValue),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books with a certain price as a JSON collection.
    /// </summary>
    /// <param name="filterValue">Optional text value that genre of the books must contain.</param>
    [HttpGet("price/{filterValue?}")]
    public JsonResult GetAllBooks_ByPrice(double filterValue = double.MinValue)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDouble = ShouldFilterByDouble(filterValue),
            FilterByDoubleValue = filterValue,
            SortResultByFieldType = ReadBooksRequest.FieldType.Numeric,
            SortResult = !ShouldFilterByDouble(filterValue)
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    private static bool ShouldFilterByDouble(double filterValue)
        => filterValue > double.MinValue;

    /// <summary>
    /// Returns all books between two double values as a JSON collection sorted by price.
    /// Example: price/5.95&10
    /// </summary>
    /// <param name="lowerPrice"></param>
    /// <param name="higherPrice"></param>
    [HttpGet("price/{lowerPrice}&{higherPrice}")]
    public JsonResult GetAllBooks_ByPrice(double lowerPrice, double higherPrice)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDouble = true,
            FilterByDoubleValue = lowerPrice < higherPrice ? lowerPrice : higherPrice,
            FilterByDoubleValue2 = lowerPrice < higherPrice ? higherPrice : lowerPrice,
            SortResultByFieldType = ReadBooksRequest.FieldType.Numeric
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by published date.
    /// </summary>
    [HttpGet("published")]
    public JsonResult GetAllBooks_ByPublishDate()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.PublishDate),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns books as a JSON collection sorted by description.
    /// </summary>
    /// <param name="filterValue">Optional text value that description of the books must contain.</param>
    [HttpGet("description/{filterValue?}")]
    public JsonResult GetAllBooks_ByDescription(string filterValue = "")
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Description),
            FilterByText = ShouldFilterByText(filterValue),
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    private static bool ShouldFilterByText(string filterValue)
    {
        return !string.IsNullOrWhiteSpace(filterValue);
    }
}
