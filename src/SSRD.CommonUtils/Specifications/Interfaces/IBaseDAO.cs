using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.CommonUtils.Specifications.Interfaces
{
    public interface IBaseDAO<TEntity>
        where TEntity : class
    {
        Task<bool> Exist<TData>(IBaseSpecification<TEntity, TData> baseSpecification);

        Task<TData> SingleOrDefault<TData>(IBaseSpecification<TEntity, TData> baseSpecification);
        Task<TData> FirstOrDefault<TData>(IBaseSpecification<TEntity, TData> baseSpecification);
        Task<List<TData>> Get<TData>(IBaseSpecification<TEntity, TData> baseSpecification);

        Task<int> Count<TData>(IBaseSpecification<TEntity, TData> baseSpecification);
        Task<TValue> Max<TValue>(IBaseSpecification<TEntity, TValue> baseSpecification);
        Task<TValue> Min<TValue>(IBaseSpecification<TEntity, TValue> baseSpecification);

        Task<bool> Add(TEntity entity);
        Task<bool> AddRange(IEnumerable<TEntity> entities);

        Task<bool> Update(TEntity entity);
        Task<bool> UpdateRange(IEnumerable<TEntity> entities);

        Task<bool> Remove(TEntity entity);
        Task<bool> RemoveRange(IEnumerable<TEntity> entities);
    }
}
