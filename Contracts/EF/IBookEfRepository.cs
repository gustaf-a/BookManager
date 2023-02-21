using Entities.ModelsEf;
using Shared;

namespace Contracts.EF;

public interface IBookEfRepository
{
    void CreateBook(BookEf book);
    Task<IEnumerable<BookEf>> GetBooks(ReadBooksRequest readBooksRequest, bool trackChanges);
}
