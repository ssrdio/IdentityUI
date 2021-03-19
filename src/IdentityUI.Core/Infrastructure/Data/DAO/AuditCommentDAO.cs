using Microsoft.EntityFrameworkCore;
using SSRD.Audit.Data;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.DAO
{
    internal class AuditCommentDAO : AuditBaseDAO<AuditCommentEntity>, IAuditCommentDAO
    {
        public AuditCommentDAO(IdentityDbContext dbContext) : base(dbContext)
        {
        }

        public Task<List<AuditCommentData>> GetComments(long auditId)
        {
            IQueryable<AuditCommentData> query =
                from comments in _dbContext.AuditComment.Where(x => x.AuditId == auditId)
                join user in _dbContext.Users on comments.UserId equals user.Id
                orderby comments.Created descending
                select new AuditCommentData(comments.Comment, comments.Created, comments.UserId, user.UserName);

            return query.ToListAsync();
        }
    }
}
