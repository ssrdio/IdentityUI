using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class, IBaseEntity
    {
        bool Exist(IBaseSpecification<TEntity> specification);

        TEntity Get(IBaseSpecification<TEntity> specification);
        TEntity SingleOrDefault(IBaseSpecification<TEntity> specification);
        TData Get<TData>(ISelectSpecification<TEntity, TData> specification);
        TData SingleOrDefault<TData>(ISelectSpecification<TEntity, TData> specification);

        List<TData> GetList<TData>(ISelectSpecification<TEntity, TData> specification);
        PaginatedData<TData> GetPaginated<TData>(IPaginationSpecification<TEntity, TData> specification);

        bool Add(TEntity entity);
        bool Update(TEntity entity);
        bool Remove(TEntity entity);
    }
}
