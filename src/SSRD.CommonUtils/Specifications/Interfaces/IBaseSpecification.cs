using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SSRD.CommonUtils.Specifications.Interfaces
{
    public interface IBaseSpecification<TEntity, TData>
    {
        List<Expression<Func<TEntity, bool>>> Filters { get; }
        List<Expression<Func<TEntity, object>>> Includes { get; }

        Expression<Func<TEntity, object>> OrderBy { get; }
        OrderByTypes OrderByType { get; }

        Expression<Func<TEntity, TData>> Select { get; }

        Expression<Func<TData, object>> OrderByAfterSelect { get; }
        OrderByTypes OrderByTypeAfterSelect { get; }

        bool Distinct { get; }

        int Skip { get; }
        int Take { get; }
        bool Paginate { get; }

        bool IgnoreQueryFilters { get; }
    }
}
