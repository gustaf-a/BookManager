using BookApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/books")]
public class BookController : Controller
{
    [HttpGet]
    public JsonResult GetAllBooks()
    {
        var books = new List<Book>
        {
            new Book{ Id = "B1", Author = "Testerson, TestName", Title = "Testers runbook to the galaxy", Description = "Boring book which tests your patience.", Genre = "Testing", Price = 1.99, PublishedDate = DateTime.Now.AddYears(-5) },
            new Book{ Id = "B2", Author = "Testerson2, TestName2", Title = "Testers runbook to the galaxy 2", Description = "Boring new book which tests your patience again.", Genre = "Testing", Price = 3.99, PublishedDate = DateTime.Now.AddYears(-2) },
        };

        return Json(books);
    }
}
