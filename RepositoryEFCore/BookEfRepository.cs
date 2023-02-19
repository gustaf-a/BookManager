using Contracts.EF;
using Entities.ModelsEf;

namespace RepositoryEFCore;

public class BookEfRepository : RepositoryBase<BookEf>, IBookEfRepository
{
    public BookEfRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public Task<IEnumerable<BookEf>> GetAllBooks(bool trackChanges)
        => Task.FromResult(GetAllBooksInternal(trackChanges));

    private IEnumerable<BookEf> GetAllBooksInternal(bool trackChanges)
        => FindAll(trackChanges)
            .OrderBy(b => b.Id)
            .ToList();
}

