using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    [Obsolete("Use SSRD.CommonUtils.Specifications.BaseSpecification")]
    public class BaseSpecification<TEntity> : IBaseSpecification<TEntity> where TEntity : class, IBaseEntity
    {
        public List<Expression<Func<TEntity, bool>>> Filters { get; } = new List<Expression<Func<TEntity, bool>>>();
        public List<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

        public  BaseSpecification()
        {

        }

        public virtual void AddFilter(Expression<Func<TEntity, bool>> filter)
        {
            Filters.Add(filter);
        }

        public virtual void AddInclude(Expression<Func<TEntity, object>> include)
        {
            Includes.Add(include);
        }
    }
}
