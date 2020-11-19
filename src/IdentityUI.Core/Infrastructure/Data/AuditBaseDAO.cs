using SSRD.Audit.Data;
using SSRD.CommonUtils.Specifications.DAO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    internal class AuditBaseDAO<TEntity> : BaseSpecificationDAO<IdentityDbContext, TEntity>
        where TEntity : class, IAuditBaseEntity
    {
        public AuditBaseDAO(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
