using SSRD.CommonUtils.Specifications.DAO;
using SSRD.IdentityUI.Core.Data.Entities;

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    internal class IdentityUIBaseDAO<TEntity> : BaseSpecificationDAO<IdentityDbContext, TEntity> 
        where TEntity : class, IIdentityUIEntity
    {
        public IdentityUIBaseDAO(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
