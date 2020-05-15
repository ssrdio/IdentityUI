using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Data.Specification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Repository
{
    internal class SessionRepository : BaseRepository<SessionEntity>, ISessionRepository
    {
        public SessionRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<bool> Remove(List<SessionEntity> sessions)
        {
            _context.RemoveRange(sessions);

            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}
