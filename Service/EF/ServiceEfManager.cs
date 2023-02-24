using AutoMapper;
using Contracts;
using Contracts.EF;
using Service.Contracts;

namespace Service.EF;

public class ServiceEfManager : IServiceManager
{
    private readonly Lazy<IBookService> _bookEfService;

    public ServiceEfManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
    {
        _bookEfService = new Lazy<IBookService>(() =>
            new BookEfService(repositoryManager, logger, mapper));
    }

    public IBookService BookService => _bookEfService.Value;
}
