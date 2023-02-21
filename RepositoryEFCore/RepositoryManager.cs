using Contracts.EF;

namespace RepositoryEFCore;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;

    private readonly Lazy<IBookEfRepository> _bookRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;

        _bookRepository = new Lazy<IBookEfRepository>(() => new BookEfRepository(repositoryContext));
    }

    public IBookEfRepository Book => _bookRepository.Value;

    public async Task SaveAsync()
        => await _repositoryContext.SaveChangesAsync();
}
