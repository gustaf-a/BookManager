using Shared;

namespace Service.Contracts;

public interface IBookService
{
    public Task<BookDto> CreateBook(BookDto bookDto);
    public Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<bool> UpdateBook(BookDto bookDto, string bookId);
    public Task<bool> DeleteBook(string bookId);
}
