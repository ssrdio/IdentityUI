using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Infrastructure.Data.Extensions;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRD.IdentityUI.Core.Data.Enums.Entity;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Repository
{
    internal class RoleRepository : BaseRepository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(IdentityDbContext context) : base(context)
        {
        }

        public List<RoleEntity> GetAssigned(string userId)
        {
            return _context.UserRoles
                .Where(x => x.UserId == userId)
                .Where(x => x.Role.Type == RoleTypes.Global)
                .Select(x => x.Role)
                .ToList();
        }

        public List<RoleEntity> GetAvailable(string userId)
        {
            List<string> assingedRoles = _context.UserRoles
                .Where(x => x.UserId == userId)
                .Where(x => x.Role.Type == RoleTypes.Global)
                .Select(x => x.RoleId)
                .ToList();

            List<RoleEntity> roles = _context.Roles
                .Where(x => !assingedRoles.Contains(x.Id))
                .Where(x => x.Type == RoleTypes.Global)
                .ToList();

            return roles;
        }
    }
}
