using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Models.Constants;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Seeders
{
    internal class UserSeeder
    {
        private readonly IdentityDbContext _context;
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(IdentityDbContext context, UserManager<AppUserEntity> userManager, ILogger<UserSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Seed(string emailDomain, string password)
        {
            _logger.LogInformation($"Seeding sample users");

            AppUserEntity[] sampleUsers = new AppUserEntity[]
            {
                    new AppUserEntity(
                        userName: $"carson.alexander@{emailDomain}",
                        email: $"carson.alexander@{emailDomain}",
                        firstName:"Carson",
                        lastName: "Alexander",
                        emailConfirmed: true,
                        enabled: true),
                    new AppUserEntity(
                        userName: $"merdith.alonso@{emailDomain}",
                        email: $"merdith.alonso@{emailDomain}",
                        firstName:"Meredith",
                        lastName: "Alonso",
                        emailConfirmed: true,
                        enabled: true),
                    new AppUserEntity(
                        userName: $"arturo.anad@{emailDomain}",
                        email: $"arturo.anad@{emailDomain}",
                        firstName: "Arturo",
                        lastName: "Anand",
                        emailConfirmed: true,
                        enabled: true),
                    new AppUserEntity(
                        userName: $"gytis.barzdukas@{emailDomain}",
                        email: $"gytis.barzdukas@{emailDomain}",
                        firstName: "Gytis",
                        lastName: "Barzdukas",
                        emailConfirmed: true,
                        enabled: true),

                    new AppUserEntity(
                        userName: $"yan.li@{emailDomain}",
                        email: $"yan.li@{emailDomain}",
                        firstName: "Yan",
                        lastName: "Li",
                        emailConfirmed: true,
                        enabled: true),
                    new AppUserEntity(
                        userName: $"peggy.justice@{emailDomain}",
                        email: $"peggy.justice@{emailDomain}",
                        firstName: "Peggy",
                        lastName: "Justice",
                        emailConfirmed: true,
                        enabled: true),
                    new AppUserEntity(
                        userName: $"laura.norman@{emailDomain}",
                        email: $"laura.norman@{emailDomain}",
                        firstName: "Laura",
                        lastName: "Norman",
                        emailConfirmed: true,
                        enabled: true),
                    new AppUserEntity(
                        userName: $"nino.olivetto@{emailDomain}",
                        email: $"nino.olivetto@{emailDomain}",
                        firstName: "Nino",
                        lastName: "Olivetto",
                        emailConfirmed: true,
                        enabled: true),
            };

            foreach (var user in sampleUsers)
            {
                IdentityResult result = await _userManager.CreateAsync(user, password);
                if(!result.Succeeded)
                {
                    _logger.LogError($"Failed to seed user. Username {user.UserName}");
                    continue;
                }
            }

            _logger.LogInformation($"Identity database was Seeded");
        }
    }
}
