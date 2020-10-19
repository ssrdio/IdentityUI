using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    [Obsolete("Use Specifications.Interfaces.IBaseDAO")]
    public interface IBaseRepositoryAsync<TEntity> where TEntity : class, IBaseEntity
    {
        Task<bool> Exist(IBaseSpecification<TEntity> specification);

        Task<TEntity> FirstOrDefault(IBaseSpecification<TEntity> specification);
        Task<TEntity> SingleOrDefault(IBaseSpecification<TEntity> specification);

        Task<TData> FirstOrDefault<TData>(ISelectSpecification<TEntity, TData> specification);
        Task<TData> SingleOrDefault<TData>(ISelectSpecification<TEntity, TData> specification);

        Task<List<TData>> GetList<TData>(ISelectSpecification<TEntity, TData> specification);
        Task<PaginatedData<TData>> GetPaginated<TData>(IPaginationSpecification<TEntity, TData> specification);

        Task<bool> Add(TEntity entity);
        Task<bool> AddRange(IEnumerable<TEntity> entities);

        Task<bool> Update(TEntity entity);

        Task<bool> Remove(TEntity entity);
        Task<bool> RemoveRange(IEnumerable<TEntity> entities);
    }
}
