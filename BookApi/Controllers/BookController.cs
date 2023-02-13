using BookApi.Data;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;

[ApiController]
[Produces("application/json")] // Ensures JSON is returned
[Route("api/books")]
public class BookController : Controller
{
    private readonly IBookService _bookService;

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
            FilterByDoubleValue = filterValue,
            SortResultByFieldType = ReadBooksRequest.FieldType.Numeric,
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books between two double values as a JSON collection sorted by price.
    /// Example: "price/5.95&10"
    /// </summary>
    /// <param name="lowerPrice">Lower value when searching a range of prices.</param>
    /// <param name="higherPrice">Higher value when searching a range of prices.</param>
    [HttpGet("price/{lowerPrice}&{higherPrice}")]
    public JsonResult GetAllBooks_ByPrice(double lowerPrice, double higherPrice)
    {
        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDoubleValue = lowerPrice < higherPrice ? lowerPrice : higherPrice,
            FilterByDoubleValue2 = lowerPrice < higherPrice ? higherPrice : lowerPrice,
            SortResultByFieldType = ReadBooksRequest.FieldType.Numeric
        };

        return Json(_bookService.GetBooks(readBooksRequest));
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
    public JsonResult GetAllBooks_ByPublishDate(int year = int.MinValue, int month = int.MinValue, int day = int.MinValue)
    {
        var filterByDatePrecision = ReadBooksRequest.FindDatePrecision(year, month, day);

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.PublishDate),
            SortResultByFieldType = ReadBooksRequest.FieldType.Date,
            FilterByDateValue = filterByDatePrecision.GetDateOnly(year, month, day),
            FilterByDatePrecision = filterByDatePrecision
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
            SortResultByFieldType = ReadBooksRequest.FieldType.Text,
            FilterByTextValue = filterValue
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }
}
