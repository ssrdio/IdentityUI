using SSRD.IdentityUI.Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Data
{
    public interface IAuditCommentDAO
    {
        Task<List<AuditCommentData>> GetComments(long auditId);
    }
}
