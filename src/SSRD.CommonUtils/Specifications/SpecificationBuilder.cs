using SSRD.CommonUtils.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SSRD.CommonUtils.Specifications
{
    public class SpecificationBuilder<TEntity> : IBaseSpecificationBuilder<TEntity>
    {
        private readonly List<Expression<Func<TEntity, bool>>> _filters = new List<Expression<Func<TEntity, bool>>>();
        private readonly List<Expression<Func<TEntity, object>>> _includes = new List<Expression<Func<TEntity, object>>>();

        private readonly Expression<Func<TEntity, object>> _orderBy = null;
        private readonly OrderByTypes _orderByType = OrderByTypes.Dessending;

        private readonly int _skip = 0;
        private readonly int _take = 0;
        private readonly bool _paginate = false;

        private readonly bool _ignoreQueryFilters = false;

        public SpecificationBuilder()
        {
        }

        private SpecificationBuilder(
            List<Expression<Func<TEntity, bool>>> filters,
            List<Expression<Func<TEntity, object>>> includes,
            Expression<Func<TEntity, object>> orderBy,
            OrderByTypes orderByType,
            int skip,
            int take,
            bool paginate,
            bool ignoreQueryFilters)
        {
            _filters = filters;
            _includes = includes;
            _orderBy = orderBy;
            _orderByType = orderByType;
            _skip = skip;
            _take = take;
            _paginate = paginate;
            _ignoreQueryFilters = ignoreQueryFilters;
        }

        public IBaseSpecification<TEntity, TEntity> Build()
        {
            return new BaseSpecification<TEntity, TEntity>(
                _filters,
                _includes,
                _orderBy,
                _orderByType,
                x => x,
                false,
                null,
                OrderByTypes.Dessending,
                _skip,
                _take,
                _paginate,
                _ignoreQueryFilters);
        }

        public IBaseSpecificationBuilder<TEntity> IgnoreQueryFilters()
        {
            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: _orderByType,
                orderBy: _orderBy,
                skip: _skip,
                take: _take,
                paginate: _paginate,
                ignoreQueryFilters: true);
        }

        public IBaseSpecificationBuilder<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            _includes.Add(expression);

            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: _orderByType,
                orderBy: _orderBy,
                skip: _skip,
                take: _take,
                paginate: _paginate,
                ignoreQueryFilters: _ignoreQueryFilters);
        }

        public IBaseSpecificationBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> expression, OrderByTypes orderBy)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: orderBy,
                orderBy: expression,
                skip: _skip,
                take: _take,
                paginate: _paginate,
                ignoreQueryFilters: _ignoreQueryFilters);
        }

        public IBaseSpecificationBuilder<TEntity> OrderByAssending(Expression<Func<TEntity, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: OrderByTypes.Assending,
                orderBy: expression,
                skip: _skip,
                take: _take,
                paginate: _paginate,
                ignoreQueryFilters: _ignoreQueryFilters);
        }

        public IBaseSpecificationBuilder<TEntity> OrderByDessending(Expression<Func<TEntity, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: OrderByTypes.Dessending,
                orderBy: expression,
                skip: _skip,
                take: _take,
                paginate: _paginate,
                ignoreQueryFilters: _ignoreQueryFilters);
        }

        public IBaseSpecificationBuilder<TEntity> Paginate(int start, int lenght)
        {
            if (lenght < 0)
            {
                return new SpecificationBuilder<TEntity>(
                    filters: _filters,
                    includes: _includes,
                    orderByType: _orderByType,
                    orderBy: _orderBy,
                    skip: _skip,
                    take: _take,
                    paginate: false,
                    ignoreQueryFilters: _ignoreQueryFilters);
            }

            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Can not be negative");
            }

            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: _orderByType,
                orderBy: _orderBy,
                skip: start,
                take: lenght,
                paginate: true,
                ignoreQueryFilters: _ignoreQueryFilters);
        }

        public ISelectSpecificationBuilder<TEntity, TData> Select<TData>(Expression<Func<TEntity, TData>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SelectSpecificationBuilder<TEntity, TData>(
                _filters,
                _includes,
                _orderBy,
                _orderByType,
                expression,
                false,
                null,
                OrderByTypes.Dessending,
                _skip,
                _take,
                _paginate,
                _ignoreQueryFilters);
        }

        public ISelectSpecificationBuilder<TEntity, TEntity> Select()
        {
            return new SelectSpecificationBuilder<TEntity, TEntity>(
                _filters,
                _includes,
                _orderBy,
                _orderByType,
                x => x,
                false,
                null,
                OrderByTypes.Dessending,
                _skip,
                _take,
                _paginate,
                _ignoreQueryFilters);
        }

        public IBaseSpecificationBuilder<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter), "Can not be null");
            }

            _filters.Add(filter);

            return new SpecificationBuilder<TEntity>(
                filters: _filters,
                includes: _includes,
                orderByType: _orderByType,
                orderBy: _orderBy,
                skip: _skip,
                take: _take,
                paginate: _paginate,
                ignoreQueryFilters: _ignoreQueryFilters);
        }
    }

    public class SelectSpecificationBuilder<TEntity, TData> : ISelectSpecificationBuilder<TEntity, TData>
    {
        private readonly List<Expression<Func<TEntity, bool>>> _filters = new List<Expression<Func<TEntity, bool>>>();
        private readonly List<Expression<Func<TEntity, object>>> _includes = new List<Expression<Func<TEntity, object>>>();

        private readonly Expression<Func<TEntity, object>> _orderBy = null;
        private readonly OrderByTypes _orderByType = OrderByTypes.Dessending;

        private readonly Expression<Func<TEntity, TData>> _select;

        private readonly bool _distinct = false;

        private readonly Expression<Func<TData, object>> _orderByAfterSelect = null;
        private readonly OrderByTypes _orderByTypeAfterSelect = OrderByTypes.Dessending;

        private readonly int _skip = 0;
        private readonly int _take = 0;
        private readonly bool _paginate = false;

        private readonly bool _ignoreQueryFilters = false;

        public SelectSpecificationBuilder(
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
            _filters = filters;
            _includes = includes;
            _orderBy = orderBy;
            _orderByType = orderByType;
            _select = select;
            _distinct = distinct;
            _orderByAfterSelect = orderByAfterSelect;
            _orderByTypeAfterSelect = orderByTypeAfterSelect;
            _skip = skip;
            _take = take;
            _paginate = paginate;
            _ignoreQueryFilters = ignoreQueryFilters;
        }

        public IBaseSpecification<TEntity, TData> Build() => new BaseSpecification<TEntity, TData>(
            _filters,
            _includes,
            _orderBy,
            _orderByType,
            _select,
            _distinct,
            _orderByAfterSelect,
            _orderByTypeAfterSelect,
            _skip,
            _take,
            _paginate,
            _ignoreQueryFilters);

        public ISelectSpecificationBuilder<TEntity, TData> Distinct()
        {
            return new SelectSpecificationBuilder<TEntity, TData>(
            _filters,
            _includes,
            _orderBy,
            _orderByType,
            _select,
            true,
            _orderByAfterSelect,
            _orderByTypeAfterSelect,
            _skip,
            _take,
            _paginate,
            _ignoreQueryFilters);
        }

        public ISelectSpecificationBuilder<TEntity, TData> IgnoreQueryFilters()
        {
            return new SelectSpecificationBuilder<TEntity, TData>(
            _filters,
            _includes,
            _orderBy,
            _orderByType,
            _select,
            _distinct,
            _orderByAfterSelect,
            _orderByTypeAfterSelect,
            _skip,
            _take,
            _paginate,
            true);
        }

        public ISelectSpecificationBuilder<TEntity, TData> OrderBy(Expression<Func<TData, object>> expression, OrderByTypes orderBy)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SelectSpecificationBuilder<TEntity, TData>(
                _filters,
                _includes,
                _orderBy,
                _orderByType,
                _select,
                true,
                expression,
                orderBy,
                _skip,
                _take,
                _paginate,
                _ignoreQueryFilters);
        }

        public ISelectSpecificationBuilder<TEntity, TData> OrderByAssending(Expression<Func<TData, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SelectSpecificationBuilder<TEntity, TData>(
                _filters,
                _includes,
                _orderBy,
                _orderByType,
                _select,
                true,
                expression,
                OrderByTypes.Assending,
                _skip,
                _take,
                _paginate,
                _ignoreQueryFilters);
        }

        public ISelectSpecificationBuilder<TEntity, TData> OrderByDessending(Expression<Func<TData, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression), "Can not be null");
            }

            return new SelectSpecificationBuilder<TEntity, TData>(
                _filters,
                _includes,
                _orderBy,
                _orderByType,
                _select,
                true,
                expression,
                OrderByTypes.Dessending,
                _skip,
                _take,
                _paginate,
                _ignoreQueryFilters);
        }

        public ISelectSpecificationBuilder<TEntity, TData> Paginate(int start, int lenght)
        {
            if (lenght < 0)
            {
                return new SelectSpecificationBuilder<TEntity, TData>(
                    filters: _filters,
                    includes: _includes,
                    orderBy: _orderBy,
                    orderByType: _orderByType,
                    select: _select,
                    distinct: _distinct,
                    orderByAfterSelect: _orderByAfterSelect,
                    orderByTypeAfterSelect: _orderByTypeAfterSelect,
                    skip: _skip,
                    take: _take,
                    paginate: false,
                    ignoreQueryFilters: _ignoreQueryFilters);
            }

            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Can not be negative");
            }

            return new SelectSpecificationBuilder<TEntity, TData>(
                filters: _filters,
                includes: _includes,
                orderBy: _orderBy,
                orderByType: _orderByType,
                select: _select,
                distinct: _distinct,
                orderByAfterSelect: _orderByAfterSelect,
                orderByTypeAfterSelect: _orderByTypeAfterSelect,
                skip: start,
                take: lenght,
                paginate: true,
                ignoreQueryFilters: _ignoreQueryFilters);
        }
    }

    public static class SpecificationBuilder
    {
        public static IBaseSpecificationBuilder<TEntity> Create<TEntity>()
        {
            SpecificationBuilder<TEntity> specificationBuilder = new SpecificationBuilder<TEntity>();

            return specificationBuilder;
        }
    }

    public interface IBaseSpecificationBuilder<TEntity>
    {
        IBaseSpecificationBuilder<TEntity> Where(Expression<Func<TEntity, bool>> filter);
        IBaseSpecificationBuilder<TEntity> OrderByAssending(Expression<Func<TEntity, object>> expression);
        IBaseSpecificationBuilder<TEntity> OrderByDessending(Expression<Func<TEntity, object>> expression);
        IBaseSpecificationBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> expression, OrderByTypes orderBy);
        IBaseSpecificationBuilder<TEntity> Paginate(int start, int lenght);
        IBaseSpecificationBuilder<TEntity> IgnoreQueryFilters();
        IBaseSpecificationBuilder<TEntity> Include(Expression<Func<TEntity, object>> expression);

        IBaseSpecification<TEntity, TEntity> Build();

        ISelectSpecificationBuilder<TEntity, TData> Select<TData>(Expression<Func<TEntity, TData>> expression);
        ISelectSpecificationBuilder<TEntity, TEntity> Select();
    }

    public interface ISelectSpecificationBuilder<TEntity, TData>
    {
        ISelectSpecificationBuilder<TEntity, TData> Distinct();

        ISelectSpecificationBuilder<TEntity, TData> OrderByAssending(Expression<Func<TData, object>> expression);
        ISelectSpecificationBuilder<TEntity, TData> OrderByDessending(Expression<Func<TData, object>> expression);
        ISelectSpecificationBuilder<TEntity, TData> OrderBy(Expression<Func<TData, object>> expression, OrderByTypes orderBy);

        ISelectSpecificationBuilder<TEntity, TData> Paginate(int start, int lenght);

        ISelectSpecificationBuilder<TEntity, TData> IgnoreQueryFilters();

        IBaseSpecification<TEntity, TData> Build();
    }
}
