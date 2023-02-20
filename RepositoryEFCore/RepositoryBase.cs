using Contracts.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RepositoryEFCore;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected RepositoryContext RepositoryContext;

    public RepositoryBase(RepositoryContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }

    // ------------------ CREATE ------------------

    public void Create(T entity)
    {
        RepositoryContext.Set<T>().Add(entity);
    }

    // ------------------ READ ------------------

    public IQueryable<T> FindAll(bool trackChanges)
        => trackChanges
            ? RepositoryContext.Set<T>()
            : RepositoryContext.Set<T>().AsNoTracking();

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
            => trackChanges
            ? RepositoryContext.Set<T>().Where(expression)
            : RepositoryContext.Set<T>().Where(expression).AsNoTracking();

    // ------------------ UPDATE ------------------

    public void Update(T entity)
    {
        RepositoryContext.Set<T>().Update(entity);
    }

    // ------------------ DELETE ------------------

    public void Delete(T entity)
    {
        RepositoryContext.Set<T>().Remove(entity);
    }
}
