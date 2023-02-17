using Shared;

namespace Service.Contracts;

public interface IBookService
{
    public Task<BookDto> CreateBook(BookDto BookDto);
    public Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<bool> UpdateBook(BookDto BookDto, string bookId);
    public Task<bool> DeleteBook(string bookId);
}
