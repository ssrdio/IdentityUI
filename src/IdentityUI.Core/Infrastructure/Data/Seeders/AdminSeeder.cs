using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Models.Constants;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Seeders
{
    internal class AdminSeeder
    {
        private readonly IdentityDbContext _context;
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly ILogger<AdminSeeder> _logger;

        public AdminSeeder(IdentityDbContext context, UserManager<AppUserEntity> userManager, ILogger<AdminSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Seed(string username, string password, List<string> roles)
        {
            if (_context.Users.Any())
            {
                _logger.LogError($"Database is not empty. Skipping admin seeding");
                return;
            }

            _logger.LogInformation("Seeding admin");

            AppUserEntity appUser = new AppUserEntity(
                userName: username,
                email: null,
                firstName: null,
                lastName: null,
                emailConfirmed: true,
                enabled: true);

            IdentityResult createAdminResult = await _userManager.CreateAsync(appUser, password);
            if (!createAdminResult.Succeeded)
            {
                _logger.LogError($"Failed to add admin. {string.Join(" ", createAdminResult.Errors.Select(x => x.Description))}");
                throw new Exception($"Failed to seed admin");
            }

            IdentityResult addToRolesResult = await _userManager.AddToRolesAsync(appUser, roles);
            if (!addToRolesResult.Succeeded)
            {
                _logger.LogError($"Failed to add admin to roles. {string.Join(" ", addToRolesResult.Errors.Select(x => x.Description))}");
                throw new Exception("Failed to seed admin roles");
            }
        }

        public Task SeedIdentityAdmin(string username, string password)
        {
            List<string> roles = new List<string> { IdentityUIRoles.IDENTITY_MANAGMENT_ROLE };

            return Seed(username, password, roles);
        }
    }
}
