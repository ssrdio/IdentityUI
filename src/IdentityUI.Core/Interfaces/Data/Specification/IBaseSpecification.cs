using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Specification
{
    public interface IBaseSpecification<TEntity> where TEntity : IBaseEntity
    {
        List<Expression<Func<TEntity, bool>>> Filters { get; }
        List<Expression<Func<TEntity, object>>> Includes { get; }
    }
}
