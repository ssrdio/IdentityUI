using SSRD.CommonUtils.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SSRD.CommonUtils.Specifications
{
    public class BaseSpecification<TEntity, TData> : IBaseSpecification<TEntity, TData>
    {
        public List<Expression<Func<TEntity, bool>>> Filters { get; }
        public List<Expression<Func<TEntity, object>>> Includes { get; }

        public Expression<Func<TEntity, object>> OrderBy { get; }
        public OrderByTypes OrderByType { get; }

        public Expression<Func<TEntity, TData>> Select { get; }

        public bool Distinct { get; }

        public Expression<Func<TData, object>> OrderByAfterSelect { get; }
        public OrderByTypes OrderByTypeAfterSelect { get; }

        public int Skip { get; }
        public int Take { get; }
        public bool Paginate { get; }

        public bool IgnoreQueryFilters { get; set; }

        public BaseSpecification(
            List<Expression<Func<TEntity, bool>>> filters,
            List<Expression<Func<TEntity, object>>> includes,
            Expression<Func<TEntity, object>> orderBy,
            OrderByTypes orderByType,
            Expression<Func<TEntity, TData>> select,
            bool distinct,
            Expression<Func<TData, object>> orderByAfterSelect,
            OrderByTypes orderByTypeAfterSelect,
            int skip,
            int take,
            bool paginate,
            bool ignoreQueryFilters)
        {
            Filters = filters;
            Includes = includes;
            OrderBy = orderBy;
            OrderByType = orderByType;
            Select = select;
            OrderByAfterSelect = orderByAfterSelect;
            OrderByTypeAfterSelect = orderByTypeAfterSelect;
            Distinct = distinct;
            Skip = skip;
            Take = take;
            Paginate = paginate;
            IgnoreQueryFilters = ignoreQueryFilters;
        }
    }
}
