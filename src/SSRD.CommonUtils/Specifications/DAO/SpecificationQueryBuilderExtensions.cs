using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;

namespace SSRD.CommonUtils.Specifications.DAO
{
    public static class SpecificationQueryBuilderExtensions
    {
        public static IQueryable<TData> ApplyBaseSpecification<TEntity, TData>(this IQueryable<TEntity> query, IBaseSpecification<TEntity, TData> baseSpecification)
            where TEntity : class
        {
            if (baseSpecification == null)
            {
                throw new ArgumentNullException(nameof(baseSpecification), "Can not be null");
            }
            
            if(baseSpecification.IgnoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (baseSpecification.Filters != null)
            {
                query = baseSpecification.Filters
                    .Aggregate(query, (x, filter) => x.Where(filter));
            }

            if (baseSpecification.Includes != null)
            {
                query = baseSpecification.Includes
                    .Aggregate(query, (x, include) => x.Include(include));
            }

            if (baseSpecification.OrderBy != null)
            {
                switch (baseSpecification.OrderByType)
                {
                    case OrderByTypes.Assending:
                        {
                            query = query.OrderBy(baseSpecification.OrderBy);

                            break;
                        }
                    case OrderByTypes.Dessending:
                        {
                            query = query.OrderByDescending(baseSpecification.OrderBy);

                            break;
                        }
                    default:
                        {
                            throw new Exception($"Unsupported orderby type {baseSpecification.OrderByType}");
                        }
                }
            }

            if (baseSpecification.Select == null)
            {
                throw new ArgumentNullException(nameof(baseSpecification.Select), "Can not be null");
            }

            IQueryable<TData> selectQuery = query.Select(baseSpecification.Select);

            if (baseSpecification.Distinct)
            {
                selectQuery = selectQuery.Distinct();
            }

            if (baseSpecification.OrderByAfterSelect != null)
            {
                switch (baseSpecification.OrderByTypeAfterSelect)
                {
                    case OrderByTypes.Assending:
                        {
                            selectQuery = selectQuery.OrderBy(baseSpecification.OrderByAfterSelect);

                            break;
                        }
                    case OrderByTypes.Dessending:
                        {
                            selectQuery = selectQuery.OrderByDescending(baseSpecification.OrderByAfterSelect);

                            break;
                        }
                    default:
                        {
                            throw new Exception($"Unsupported orderby type {baseSpecification.OrderByTypeAfterSelect}");
                        }
                }
            }

            if (baseSpecification.Paginate)
            {
                if (baseSpecification.Skip < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(baseSpecification), "Can not be negative");
                }

                if (baseSpecification.Take < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(baseSpecification), "Can not be negative");
                }

                selectQuery = selectQuery
                    .Skip(baseSpecification.Skip)
                    .Take(baseSpecification.Take);
            }

            return selectQuery;
        }
    }
}
