using BookApi.Data;

namespace BookApi.Repositories;

public interface IBookRepository
{
    public IEnumerable<Book> GetBooks();
}
