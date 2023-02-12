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
            FieldToSortBy = nameof(Book.Id),
            FilterByValue = ShouldFilterByValue(filterValue),
            Type = ReadBooksRequest.FieldType.Text,
            ValueToFilterBy = filterValue
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
            FieldToSortBy = nameof(Book.Author),
            FilterByValue = ShouldFilterByValue(filterValue),
            Type = ReadBooksRequest.FieldType.Text,
            ValueToFilterBy = filterValue
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
            FieldToSortBy = nameof(Book.Title),
            FilterByValue = ShouldFilterByValue(filterValue),
            Type = ReadBooksRequest.FieldType.Text,
            ValueToFilterBy = filterValue
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
            FieldToSortBy = nameof(Book.Genre),
            FilterByValue = ShouldFilterByValue(filterValue),
            Type = ReadBooksRequest.FieldType.Text,
            ValueToFilterBy = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by price.
    /// </summary>
    [HttpGet("price")]
    public JsonResult GetAllBooks_ByPrice()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy = nameof(Book.Price),
            Type = ReadBooksRequest.FieldType.Numeric
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
            FieldToSortBy = nameof(Book.PublishDate),
            Type = ReadBooksRequest.FieldType.Text
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
            FieldToSortBy = nameof(Book.Description),
            FilterByValue = ShouldFilterByValue(filterValue),
            Type = ReadBooksRequest.FieldType.Text,
            ValueToFilterBy = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    private static bool ShouldFilterByValue(string filterValue)
    {
        return !string.IsNullOrWhiteSpace(filterValue);
    }
}
