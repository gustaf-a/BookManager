using System.Linq.Expressions;

namespace Contracts.EF;

public interface IRepositoryBase<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="trackChanges">Greatly improves read speeds if set to false</param>
    IQueryable<T> FindAll(bool trackChanges);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="trackChanges">Greatly improves read speeds if set to false</param>
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
    bool trackChanges);

    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}
