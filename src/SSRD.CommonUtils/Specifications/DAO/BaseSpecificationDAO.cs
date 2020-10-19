using Microsoft.EntityFrameworkCore;
using SSRD.CommonUtils.Specifications.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.CommonUtils.Specifications.DAO
{
    public class BaseSpecificationDAO<TDbContext, TEntity> : IBaseDAO<TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected readonly TDbContext _dbContext;

        public BaseSpecificationDAO(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual Task<List<TData>> Get<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .ApplyBaseSpecification(baseSpecification)
                .ToListAsync();
        }

        public virtual Task<int> Count<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .ApplyBaseSpecification(baseSpecification)
                .CountAsync();
        }

        public virtual Task<TData> SingleOrDefault<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .ApplyBaseSpecification(baseSpecification)
                .SingleOrDefaultAsync();
        }

        public Task<bool> Exist<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .ApplyBaseSpecification(baseSpecification)
                .AnyAsync();
        }

        public Task<TData> FirstOrDefault<TData>(IBaseSpecification<TEntity, TData> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .ApplyBaseSpecification(baseSpecification)
                .FirstOrDefaultAsync();
        }

        public Task<TValue> Max<TValue>(IBaseSpecification<TEntity, TValue> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .ApplyBaseSpecification(baseSpecification)
                .MaxAsync();
        }

        public Task<TValue> Min<TValue>(IBaseSpecification<TEntity, TValue> baseSpecification)
        {
            return _dbContext
                .Set<TEntity>()
                .ApplyBaseSpecification(baseSpecification)
                .MinAsync();
        }

        public async Task<bool> Add(TEntity entity)
        {
            _dbContext.Add(entity);

            int changes = await _dbContext.SaveChangesAsync();

            _dbContext.Entry(entity).State = EntityState.Detached;

            return changes > 0;
        }

        public async Task<bool> AddRange(IEnumerable<TEntity> entities)
        {
            _dbContext.AddRange(entities);

            int changes = await _dbContext.SaveChangesAsync();

            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }

            return changes > 0;
        }

        public async Task<bool> Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;

            int changes = await _dbContext.SaveChangesAsync();

            _dbContext.Entry(entity).State = EntityState.Detached;

            return changes > 0;
        }

        public async Task<bool> UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbContext.UpdateRange(entities);

            int changes = await _dbContext.SaveChangesAsync();

            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }

            return changes > 0;
        }

        public async Task<bool> Remove(TEntity entity)
        {
            _dbContext.Remove(entity);

            int changes = await _dbContext.SaveChangesAsync();

            _dbContext.Entry(entity).State = EntityState.Detached;

            return changes > 0;
        }

        public async Task<bool> RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbContext.RemoveRange(entities);

            int changes = await _dbContext.SaveChangesAsync();

            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }

            return changes > 0;
        }
    }
}
