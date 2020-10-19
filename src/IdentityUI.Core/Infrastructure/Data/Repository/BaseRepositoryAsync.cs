using Microsoft.EntityFrameworkCore;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Infrastructure.Data.Extensions;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Repository
{
    internal class BaseRepositoryAsync<TEntity> : IBaseRepositoryAsync<TEntity> where TEntity : class, IBaseEntity
    {
        protected readonly IdentityDbContext _context;

        public BaseRepositoryAsync(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Added;
            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> AddRange(IEnumerable<TEntity> entities)
        {
            _context
                .Set<TEntity>()
                .AddRange(entities);

            int changes = await _context.SaveChangesAsync();
            return changes == entities.Count();
        }

        public Task<bool> Exist(IBaseSpecification<TEntity> specification)
        {
            return _context
                .Set<TEntity>()
                .ApplayBaseSpecification(specification)
                .AnyAsync();
        }

        public Task<TEntity> FirstOrDefault(IBaseSpecification<TEntity> specification)
        {
            return _context
                .Set<TEntity>()
                .AsNoTracking()
                .ApplayBaseSpecification(specification)
                .FirstOrDefaultAsync();
        }

        public Task<TData> FirstOrDefault<TData>(ISelectSpecification<TEntity, TData> specification)
        {
            return _context
                .Set<TEntity>()
                .AsNoTracking()
                .ApplaySelectSpecification(specification)
                .FirstOrDefaultAsync();
        }

        public Task<List<TData>> GetList<TData>(ISelectSpecification<TEntity, TData> specification)
        {
            return _context
                .Set<TEntity>()
                .AsNoTracking()
                .ApplaySelectSpecification(specification)
                .ToListAsync();
        }

        public async Task<PaginatedData<TData>> GetPaginated<TData>(IPaginationSpecification<TEntity, TData> specification)
        {
            List<TData> data = await _context
                .Set<TEntity>()
                .AsNoTracking()
                .ApplyPaginationSpecification(specification)
                .ToListAsync();

            int count = await _context
                .Set<TEntity>()
                .ApplayBaseSpecification(specification)
                .CountAsync();

            return new PaginatedData<TData>(
                data: data,
                count: count);
        }

        public async Task<bool> Remove(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;

            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> RemoveRange(IEnumerable<TEntity> entities)
        {
            _context
                .Set<TEntity>()
                .RemoveRange(entities);

            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public Task<TEntity> SingleOrDefault(IBaseSpecification<TEntity> specification)
        {
            return _context
                .Set<TEntity>()
                .AsNoTracking()
                .ApplayBaseSpecification(specification)
                .SingleOrDefaultAsync();
        }

        public Task<TData> SingleOrDefault<TData>(ISelectSpecification<TEntity, TData> specification)
        {
            return _context
                .Set<TEntity>()
                .AsNoTracking()
                .ApplaySelectSpecification(specification)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }
    }
}
