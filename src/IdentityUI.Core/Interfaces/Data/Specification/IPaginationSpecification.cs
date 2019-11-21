using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Specification
{
    public interface IPaginationSpecification<TEntity, TData> : IBaseSpecification<TEntity>, ISelectSpecification<TEntity, TData> where TEntity : class, IBaseEntity
    {
        int Take { get; }
        int Skip { get; }
    }
}
