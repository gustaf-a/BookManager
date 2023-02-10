using BookApi.Data;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/books")]
public class BookController : Controller
{
    private IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public JsonResult GetAllBooks()
    {
        return Json(_bookService.GetBooks());
    }
}
