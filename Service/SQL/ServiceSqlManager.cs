using Contracts;
using Service.Contracts;

namespace Service.SQL;

public class ServiceSqlManager : IServiceManager
{
    private readonly Lazy<IBookService> _bookService;

    public ServiceSqlManager(IBookRepository bookRepository, ILoggerManager logger)
    {
        _bookService = new Lazy<IBookService>(() =>
            new BookService(logger, bookRepository));
    }

    public IBookService BookService => _bookService.Value;
}
