using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRD.Audit.Data;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Extensions
{
    internal static class SpecificationExtension
    {
        public static IQueryable<TEntity> ApplayBaseSpecification<TEntity>(this IQueryable<TEntity> query, IBaseSpecification<TEntity> specification) 
            where TEntity : class, IBaseEntity
        {
            if(specification == null)
            {
                throw new ArgumentNullException($"Specification can not be null");
            }

            if (specification.Filters != null)
            {
                query = specification.Filters
                    .Aggregate(query, (current, filter) => current.Where(filter));
            }

            if (specification.Includes != null)
            {
                query = specification.Includes
                    .Aggregate(query, (current, include) => current.Include(include));
            }

            return query;

        }

        public static IQueryable<TData> ApplaySelectSpecification<TEntity, TData>(this IQueryable<TEntity> query, ISelectSpecification<TEntity, TData> specification)
            where TEntity : class, IBaseEntity
        {
            if (specification == null)
            {
                throw new ArgumentNullException($"Specification can not be null");
            }

            if (specification.Select == null)
            {
                throw new Exception("Select can not be null");
            }

            query = query.ApplayBaseSpecification(specification);

            return query.Select(specification.Select);
        }

        public static IQueryable<TData> ApplyPaginationSpecification<TEntity, TData>(this IQueryable<TEntity> query, IPaginationSpecification<TEntity, TData> specification)
            where TEntity : class, IBaseEntity
        {
            if (specification == null)
            {
                throw new ArgumentNullException($"Specification can not be null");
            }

            if (specification.Skip < 0 || specification.Take < 0)
            {
                throw new ArgumentOutOfRangeException($"Skip and Take must be greater then 0");
            }

            query = query.OrderByDescending(x => x._CreatedDate);
            IQueryable<TData> selectQuery = query.ApplaySelectSpecification(specification);

            selectQuery = selectQuery
                .Skip(specification.Skip)
                .Take(specification.Take);

            return selectQuery;
        }
    }
}
