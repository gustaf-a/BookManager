using Entities.ModelsEf;

namespace Contracts.EF;

public interface IBookEfRepository
{
    Task<IEnumerable<BookEf>> GetAllBooks(bool trackChanges);
}
