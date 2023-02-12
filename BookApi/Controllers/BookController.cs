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
    [HttpGet("id")]
    public JsonResult GetAllBooks_ById()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy= nameof(Book.Id)
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by author.
    /// </summary>
    [HttpGet("author")]
    public JsonResult GetAllBooks_ByAuthor()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy = nameof(Book.Author),
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by title.
    /// </summary>
    [HttpGet("title")]
    public JsonResult GetAllBooks_ByTitle()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy = nameof(Book.Title),
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by genre.
    /// </summary>
    [HttpGet("genre")]
    public JsonResult GetAllBooks_ByGenre()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy = nameof(Book.Genre),
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
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }

    /// <summary>
    /// Returns all books as a JSON collection sorted by description.
    /// </summary>
    [HttpGet("description")]
    public JsonResult GetAllBooks_ByDescription()
    {
        var readBooksRequest = new ReadBooksRequest
        {
            FieldToSortBy = nameof(Book.Description),
        };

        return Json(_bookService.GetBooks(readBooksRequest));
    }
}
