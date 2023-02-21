using Contracts;
using Contracts.EF;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace RepositoryEFCore;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;

    private readonly Lazy<IBookEfRepository> _bookRepository;

    public RepositoryManager(RepositoryContext repositoryContext, IOptions<DatabaseOptions> databaseOptions, IIdGenerator idGenerator)
    {
        _repositoryContext = repositoryContext;

        _bookRepository = new Lazy<IBookEfRepository>(() => new BookEfRepository(repositoryContext, databaseOptions.Value, idGenerator));
    }

    public IBookEfRepository Book => _bookRepository.Value;

    public async Task SaveAsync()
        => await _repositoryContext.SaveChangesAsync();
}
