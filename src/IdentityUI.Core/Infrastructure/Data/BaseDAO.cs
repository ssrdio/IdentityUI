using SSRD.CommonUtils.Specifications.DAO;

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    internal class BaseDAO<TEntity> : BaseSpecificationDAO<IdentityDbContext, TEntity> 
        where TEntity : class
    {
        public BaseDAO(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
