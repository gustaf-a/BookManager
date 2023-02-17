using Service.Contracts;

namespace Service.Contracts;

public interface IServiceManager
{
    IBookService BookService { get; }
}
