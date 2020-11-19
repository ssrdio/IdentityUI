using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    [Obsolete("Use SSRD.CommonUtils.Specifications.BaseSpecification")]
    public class PaginationSpecification<TEntity, TData> : SelectSpecification<TEntity, TData>, IPaginationSpecification<TEntity, TData> where TEntity : class, IBaseEntity
    {
        public int Take { get; private set; }

        public int Skip { get; private set; }

        public PaginationSpecification() : base()
        {

        }

        public virtual void AppalyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }
    }
}
