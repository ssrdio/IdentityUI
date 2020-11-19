using SSRD.CommonUtils.Specifications.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.CommonUtils.Specifications.DAO
{
    /// <summary>
    /// DAO for testing specifications. This DAO does not implemented add,remove,update methods.
    /// </summary>
    public class BaseSpecificationTestDAO<TEntity> : IBaseDAO<TEntity>
        where TEntity : class
    {
        private readonly List<TEntity> _entities;

        public BaseSpecificationTestDAO()
        {
        }

        public BaseSpecificationTestDAO(TEntity entity)
        {
            _entities = new List<TEntity>() { entity };
        }

        public BaseSpecificationTestDAO(List<TEntity> entities)
        {
            _entities = new List<TEntity>(entities);
        }

        public Task<bool> Add(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> AddRange(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> Count<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            int result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .Count();

            return Task.FromResult(result);
        }

        public Task<bool> Exist<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            bool result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .Any();

            return Task.FromResult(result);
        }

        public Task<TData> FirstOrDefault<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            TData result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .FirstOrDefault();

            return Task.FromResult(result);
        }

        public Task<List<TData>> Get<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            List<TData> result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<TValue> Max<TValue>(IBaseSpecification<TEntity, TValue> baseSpecification)
        {
            TValue result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .Max();

            return Task.FromResult(result);
        }

        public Task<TValue> Min<TValue>(IBaseSpecification<TEntity, TValue> baseSpecification)
        {
            TValue result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .Min();

            return Task.FromResult(result);
        }

        public Task<bool> Remove(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }

        public Task<TData> SingleOrDefault<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            TData result = _entities
                .AsQueryable()
                .ApplyBaseSpecification(baseSpecification)
                .SingleOrDefault();

            return Task.FromResult(result);
        }

        public Task<bool> Update(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
