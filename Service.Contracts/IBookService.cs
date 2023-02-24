using Shared;
using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IBookService
{
    public Task<BookDto> CreateBook(BookForCreationDto bookDto);
    public Task<IEnumerable<BookDto>> ReadBooks(ReadBooksRequest readBooksRequest);
    public Task<bool> UpdateBook(BookForUpdateDto bookDto, string bookId);
    public Task<bool> DeleteBook(string bookId);
}
