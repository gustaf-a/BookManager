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

    public async Task<Book> CreateBook(Book book)
    {
        return await _bookRepository.CreateBook(book);
    }

    public async Task<IEnumerable<Book>> ReadBooks(ReadBooksRequest readBooksRequest)
    {
        return await _bookRepository.ReadBooks(readBooksRequest);
    }

    public async Task<Book> UpdateBook(Book book, string bookId)
    {
        return await _bookRepository.UpdateBook(book, bookId);
    }

    public async Task<bool> DeleteBook(string bookId)
    {
        return await _bookRepository.DeleteBook(bookId);
    }

}
