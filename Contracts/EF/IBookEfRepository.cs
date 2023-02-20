using Entities.ModelsEf;
using Shared;

namespace Contracts.EF;

public interface IBookEfRepository
{
    Task<IEnumerable<BookEf>> GetBooks(ReadBooksRequest readBooksRequest, bool trackChanges);
}
