using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Models;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Seeders
{
    internal class SystemEntitySeeder
    {
        private readonly IdentityDbContext _context;
        private readonly RoleManager<RoleEntity> _roleManager;

        private readonly ILogger<SystemEntitySeeder> _logger;

        private readonly List<EmailEntity> _allEmails;

        public SystemEntitySeeder(IdentityDbContext context, RoleManager<RoleEntity> roleManager, ILogger<SystemEntitySeeder> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _logger = logger;

            _allEmails = new List<EmailEntity>();

            EmailEntity inviteEmail = new EmailEntity(
                subject: "Invitation",
                body: "You have been invited. If you wish to register <a href='{{token}}'>click here</a>",
                type: EmailTypes.Invite);
            _allEmails.Add(inviteEmail);

            EmailEntity confirmationEmail = new EmailEntity(
                subject: "Confirm your email",
                body: "Please confirm your account by <a href='{{token}}'>clicking here</a>.",
                type: EmailTypes.EmailConfirmation);
            _allEmails.Add(confirmationEmail);

            EmailEntity passwordRecovery = new EmailEntity(
                subject: "Reset Password",
                body: "Please reset your password by <a href='{{token}}'>clicking here</a>",
                type: EmailTypes.PasswordRecovery);
            _allEmails.Add(passwordRecovery);

            EmailEntity passwordWasReset = new EmailEntity(
                subject: "Password was reset",
                body: "Your password has been reset",
                type: EmailTypes.PasswordWasReset);
            _allEmails.Add(passwordWasReset);
        }

        public Task SeedIdentityUI()
        {
            return Seed(new List<PermissionSeedModel>(), new List<RoleSeedModel>());
        }

        public async Task Seed(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels)
        {
            if (_context.Permissions.Any() || _context.Roles.Any() || _context.Emails.Any())
            {
                _logger.LogError($"Database is not empty. Skipping seeding");
                return;
            }

            _logger.LogInformation($"Seeding system entities");

            permissionSeedModels.AddRange(IdentityUIPermissions.ALL_DATA);
            roleSeedModels.AddRange(IdentityUIRoles.ALL_DATA);

            SeedEmails();
            await SeedPermissionsRoles(permissionSeedModels, roleSeedModels);

        }

        public async Task SeedMissing(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels)
        {
            _logger.LogInformation($"Seeding missing system entities");

            permissionSeedModels.AddRange(IdentityUIPermissions.ALL_DATA);
            roleSeedModels.AddRange(IdentityUIRoles.ALL_DATA);

            SeedMissingEmails();
            await SeedMissingPermissinsRoles(permissionSeedModels, roleSeedModels);
        }

        private async Task SeedPermissionsRoles(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels)
        {
            IEnumerable<PermissionEntity> permissions = permissionSeedModels.Select(x => x.ToEntity());

            _context.Permissions.AddRange(permissions);
            int addPermissionChanges = _context.SaveChanges();
            if (addPermissionChanges != permissions.Count())
            {
                _logger.LogCritical($"Failed to seed Permission.");
                throw new Exception("failed_to_seed_permission");
            }

            permissions = _context.Permissions.ToList();

            List<PermissionRoleEntity> permissionRoleEntities = new List<PermissionRoleEntity>();

            foreach (RoleSeedModel role in roleSeedModels)
            {
                RoleEntity roleEntity = role.ToEntity();

                IdentityResult createRoleResult = await _roleManager.CreateAsync(roleEntity);
                if (!createRoleResult.Succeeded)
                {
                    _logger.LogCritical($"Failed to seed Role. RoleName {role.Name}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                    throw new Exception($"Failed to seed Role.");
                }

                IEnumerable<PermissionEntity> validPermissions = permissions
                    .Where(x => role.Permissions.Contains(x.Name));

                if (validPermissions.Count() != role.Permissions.Count)
                {
                    _logger.LogCritical($"Missing role permission. Role: {role.Name}");
                    throw new Exception("Missing role permission");
                }

                IEnumerable<PermissionRoleEntity> rolePermissions = validPermissions
                    .Select(x => new PermissionRoleEntity(
                        permissionId: x.Id,
                        roleId: roleEntity.Id));

                permissionRoleEntities.AddRange(rolePermissions);
            }

            _context.PermissionRole.AddRange(permissionRoleEntities);
            int addPermissionRoleChanges = _context.SaveChanges();
            if (addPermissionRoleChanges != permissionRoleEntities.Count())
            {
                _logger.LogCritical($"Failed to seed role permissions");
                throw new Exception("Failed to seed role permissions");
            }
        }

        private async Task SeedMissingPermissinsRoles(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels)
        {
            List<PermissionEntity> existingPermissions = _context.Permissions.ToList();

            IEnumerable<PermissionEntity> missingPermissions = permissionSeedModels
                .Where(x => !existingPermissions.Select(c => c.Name.ToUpper()).Contains(x.Name.ToUpper()))
                .Select(x => x.ToEntity());

            _context.Permissions.AddRange(missingPermissions);
            int addPermissionChanges = _context.SaveChanges();
            if (existingPermissions.Count + addPermissionChanges != permissionSeedModels.Count())
            {
                _logger.LogCritical($"Failed to seed missing Permission");
                throw new Exception("Failed to seed missing Permission");
            }

            existingPermissions = _context.Permissions.ToList();

            List<PermissionRoleEntity> permissionRoleEntities = new List<PermissionRoleEntity>();

            foreach (RoleSeedModel role in roleSeedModels)
            {
                RoleEntity roleEntity = await _roleManager.FindByNameAsync(role.Name);
                if (roleEntity == null)
                {
                    roleEntity = role.ToEntity();

                    IdentityResult createRoleResult = await _roleManager.CreateAsync(roleEntity);
                    if (!createRoleResult.Succeeded)
                    {
                        _logger.LogCritical($"Failed to seed missing Role. RoleName {role.Name}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                        throw new Exception($"Failed to seed missing Role.");
                    }

                    IEnumerable<PermissionEntity> validPermissions = existingPermissions
                        .Where(x => role.Permissions.Contains(x.Name));

                    if (validPermissions.Count() != role.Permissions.Count)
                    {
                        _logger.LogCritical($"Missing role permission. Role: {role.Name}");
                        throw new Exception("Missing role permission");
                    }

                    IEnumerable<PermissionRoleEntity> rolePermissions = validPermissions
                        .Select(x => new PermissionRoleEntity(
                            permissionId: x.Id,
                            roleId: roleEntity.Id));

                    permissionRoleEntities.AddRange(rolePermissions);
                }
                else
                {
                    List<string> rolePermissions = _context.PermissionRole
                        .Where(x => x.Role.NormalizedName == role.Name.ToUpper())
                        .Select(x => x.Permission.Name)
                        .ToList();

                    IEnumerable<string> missingRolePermissions = role.Permissions.Where(x => !rolePermissions.Contains(x));

                    IEnumerable<PermissionEntity> validPermissions = existingPermissions
                        .Where(x => missingRolePermissions.Contains(x.Name));

                    if (rolePermissions.Count + validPermissions.Count() < role.Permissions.Count)
                    {
                        _logger.LogCritical($"Missing role permission. Role: {role.Name}");
                        throw new Exception("Missing role permission");
                    }

                    IEnumerable<PermissionRoleEntity> permissionRoles = validPermissions
                        .Select(x => new PermissionRoleEntity(
                            permissionId: x.Id,
                            roleId: roleEntity.Id));

                    permissionRoleEntities.AddRange(permissionRoles);
                }
            }

            _context.PermissionRole.AddRange(permissionRoleEntities);
            int addPermissionRoleChanges = _context.SaveChanges();
            if (addPermissionRoleChanges != permissionRoleEntities.Count())
            {
                _logger.LogCritical($"Failed to seed role permissions");
                throw new Exception("Failed to seed role permissions");
            }
        }

        private void SeedEmails()
        {
            _context.Emails.AddRange(_allEmails);
            int changes = _context.SaveChanges();
            if (changes != _allEmails.Count)
            {
                _logger.LogCritical($"Failed to seed emails");
                throw new Exception("Failed to seed emails");
            }
        }

        private void SeedMissingEmails()
        {
            List<EmailTypes> emailTypes = _context.Emails
                .Select(x => x.Type)
                .ToList();

            IEnumerable<EmailEntity> missingEmails = _allEmails.Where(x => !emailTypes.Contains(x.Type));

            _context.Emails.AddRange(missingEmails);
            int changes = _context.SaveChanges();
            if (emailTypes.Count + changes < _allEmails.Count)
            {
                _logger.LogCritical($"Failed to seed emails");
                throw new Exception("Failed to seed emails");
            }
        }
    }
}
