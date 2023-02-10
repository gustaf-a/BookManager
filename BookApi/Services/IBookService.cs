using BookApi.Data;

namespace BookApi.Services;

public interface IBookService
{
    public IEnumerable<Book> GetBooks();
}
