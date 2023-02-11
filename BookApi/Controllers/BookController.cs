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
        return Json(_bookService.GetBooks());
    }
}
