namespace Contracts;

/// <summary>
/// Creates instances of repositories to facilitate handling multiple classes and controls when to save.
/// Allows for multiple modifications with one save for performance and atomic transactions.
/// </summary>
public interface IRepositoryManager
{
    public IBookRepository Book { get; }

    public void Save();
}