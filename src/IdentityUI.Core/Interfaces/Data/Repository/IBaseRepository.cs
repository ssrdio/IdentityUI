using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    [Obsolete("Use Specifications.Interfaces.IBaseDAO")]
    public interface IBaseRepository<TEntity> where TEntity : class, IBaseEntity
    {
        bool Exist(IBaseSpecification<TEntity> specification);

        TEntity FirstOrDefault(IBaseSpecification<TEntity> specification);
        TEntity SingleOrDefault(IBaseSpecification<TEntity> specification);

        TData FirstOrDefault<TData>(ISelectSpecification<TEntity, TData> specification);
        TData SingleOrDefault<TData>(ISelectSpecification<TEntity, TData> specification);

        List<TData> GetList<TData>(ISelectSpecification<TEntity, TData> specification);
        PaginatedData<TData> GetPaginated<TData>(IPaginationSpecification<TEntity, TData> specification);

        bool Add(TEntity entity);
        bool AddRange(IEnumerable<TEntity> entities);
        
        bool Update(TEntity entity);

        bool Remove(TEntity entity);
        bool RemoveRange(IEnumerable<TEntity> entities);
    }
}
