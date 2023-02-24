using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service.SQL;

public class ServiceSqlManager : IServiceManager
{
    private readonly Lazy<IBookService> _bookService;

    public ServiceSqlManager(IBookRepository bookRepository, ILoggerManager logger, IMapper mapper)
    {
        _bookService = new Lazy<IBookService>(() =>
            new BookService(logger, bookRepository, mapper));
    }

    public IBookService BookService => _bookService.Value;
}
