using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityUI.Dev.Services
{
    public class UserSeeder
    {
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly IBaseRepository<SessionEntity> _sessionRepository;


        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(UserManager<AppUserEntity> userManager, IBaseRepository<SessionEntity> sessionRepository, ILogger<UserSeeder> logger)
        {
            _userManager = userManager;
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Dont forget do disable AddAuditInfo/>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task Seed(DateTime from, DateTime to)
        {
            if(to < from)
            {
                throw new Exception("To can not be smaller than from");
            }

            Random random = new Random();

            List<SessionEntity> sessionEntities = new List<SessionEntity>();

            for(DateTime date = from; date < to; date = date.AddDays(1))
            {
                int usersForDay = random.Next(10, 50);

                for(int i = 0; i < usersForDay; i++)
                {
                    AppUserEntity appUserEntity = new AppUserEntity(
                        userName: Guid.NewGuid().ToString(),
                        email: Guid.NewGuid().ToString(),
                        firstName: "seed",
                        lastName: "seed",
                        emailConfirmed: random.Next(0, 100) < 10 ? false : true,
                        enabled: random.Next(0, 100) < 5 ? false : true);

                    appUserEntity._CreatedDate = date;

                    IdentityResult result = await _userManager.CreateAsync(appUserEntity);

                    if(random.Next(0, 100) < 80)
                    {
                        SessionEntity sessionEntity = new SessionEntity(
                            "",
                            appUserEntity.Id,
                            "");

                        sessionEntities.Add(sessionEntity);
                    }
                }
            }

            _sessionRepository.AddRange(sessionEntities);
        }
    }
}
