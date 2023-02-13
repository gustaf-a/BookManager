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

    public Book CreateBook(Book book)
    {
        return _bookRepository.CreateBook(book);
    }

    public IEnumerable<Book> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        return _bookRepository.ReadBooks(readBooksRequest);
    }

    public Book UpdateBook(Book book, string bookId)
    {
        return _bookRepository.UpdateBook(book, bookId);
    }

    public bool DeleteBook(string bookId)
    {
        return _bookRepository.DeleteBook(bookId);
    }

}
