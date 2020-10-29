using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    [Obsolete("Use SSRD.CommonUtils.Specifications.BaseSpecification")]
    public class SelectSpecification<TEntity, TData> : BaseSpecification<TEntity>, ISelectSpecification<TEntity, TData> where TEntity : class, IBaseEntity
    {
        public Expression<Func<TEntity, TData>> Select { get; private set; }

        public SelectSpecification() : base()
        {

        }

        public void AddSelect(Expression<Func<TEntity, TData>> expression)
        {
            Select = expression;
        }
    }
}
