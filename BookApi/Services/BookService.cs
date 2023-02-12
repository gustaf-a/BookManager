using BookApi.Data;
using BookApi.Repositories;

namespace BookApi.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public IEnumerable<Book> GetBooks(ReadBooksRequest readBooksRequest)
    {
        return _bookRepository.GetBooks(readBooksRequest);
    }
}
