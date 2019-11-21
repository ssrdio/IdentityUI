using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Repository
{
    internal class UserRepository : BaseRepository<AppUserEntity>, IUserRepository
    {
        public UserRepository(IdentityDbContext context) : base(context)
        {
        }

        public int GetActiveUsersCount()
        {
            return _context.Sessions
                .Select(x => x.UserId)
                .Distinct()
                .Count();
        }

        public int GetDisabledUsersCount()
        {
            return _context.Users
                .Where(x => x.Enabled == false)
                .Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">Is inclusive</param>
        /// <param name="to">Is exclusive</param>
        public List<GroupedCountData> GetRegistrations(DateTimeOffset from, DateTimeOffset to)
        {
            return _context.Users
                .Where(x => x._CreatedDate != null)
                .Where(x => x._CreatedDate >= from)
                .Where(x => x._CreatedDate < to)
                .Select(x => new {
                    x.Id,
                    x._CreatedDate
                })
                .ToList() //TO DO: fix so that group by will not be evaluetad localy
                .GroupBy(x => new { x._CreatedDate.Value.Year, x._CreatedDate.Value.Month, x._CreatedDate.Value.Day })
                .Select(x => new GroupedCountData(
                    new DateTimeOffset(new DateTime(x.Key.Year, x.Key.Month, x.Key.Day), TimeSpan.FromHours(0)),
                    x.Count()))
                .ToList();
        }

        public int GetUnconfirmedUsersCount()
        {
            return _context.Users
                .Where(x => x.EmailConfirmed == false)
                .Count();
        }

        public int GetUsersCount()
        {
            return _context.Users
                .Count();
        }
    }
}
