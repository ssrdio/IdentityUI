using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    public interface ISessionRepository : IBaseRepository<SessionEntity>
    {
        bool Remove(SessionEntity session);
        Task<bool> Remove(List<SessionEntity> sessions);
    }
}
