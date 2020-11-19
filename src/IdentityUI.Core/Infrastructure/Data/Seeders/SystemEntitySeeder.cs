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
using Microsoft.EntityFrameworkCore;

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

            EmailEntity twoFactorToken = new EmailEntity(
                subject: "Two Factor Authentication",
                body: "Authentication code: {{token}}",
                type: EmailTypes.TwoFactorAuthenticationToken);
            _allEmails.Add(twoFactorToken);
        }

        public Task SeedIdentityUI()
        {
            return Seed(new List<PermissionSeedModel>(), new List<RoleSeedModel>());
        }

        public async Task Seed(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels)
        {
            _logger.LogInformation($"Seeding system entities");

            permissionSeedModels.AddRange(IdentityUIPermissions.ALL_DATA);
            roleSeedModels.AddRange(IdentityUIRoles.ALL_DATA);

            SeedMissingEmails();

            await SeedRoles(roleSeedModels);
            SeedPermissions(permissionSeedModels);
            SeedRolePermissions(roleSeedModels);
            SeedRoleAssigments(roleSeedModels);
        }

        [Obsolete("Use Seed")]
        public async Task SeedMissing(List<PermissionSeedModel> permissionSeedModels, List<RoleSeedModel> roleSeedModels)
        {
            _logger.LogInformation($"Seeding missing system entities");

            permissionSeedModels.AddRange(IdentityUIPermissions.ALL_DATA);
            roleSeedModels.AddRange(IdentityUIRoles.ALL_DATA);

            SeedMissingEmails();

            await SeedRoles(roleSeedModels);
            SeedPermissions(permissionSeedModels);
            SeedRolePermissions(roleSeedModels);
            SeedRoleAssigments(roleSeedModels);
        }

        private void SeedPermissions(List<PermissionSeedModel> permissionSeedModels)
        {
            List<PermissionEntity> existingPermissions = _context.Permissions.ToList();

            IEnumerable<PermissionEntity> missingPermissions = permissionSeedModels
                .Where(x => !existingPermissions.Any(c => c.Name.ToUpper() == x.Name.ToUpper()))
                .Select(x => x.ToEntity());

            _context.Permissions.AddRange(missingPermissions);
            int addPermissionChanges = _context.SaveChanges();
            if(addPermissionChanges != missingPermissions.Count())
            {
                _logger.LogCritical($"Failed to seed Permission.");
                throw new Exception("Failed to seed Permission");
            }

            existingPermissions = _context.Permissions.ToList();
            foreach (PermissionSeedModel permission in permissionSeedModels)
            {
                bool exists = existingPermissions
                    .Where(x => x.Name == permission.Name)
                    .Any();
                if (!exists)
                {
                    _logger.LogCritical($"Failed to seed Permission. Permission name {permission.Name}");
                    throw new Exception("Failed to seed Permission");
                }
            }
        }

        public async Task SeedRoles(List<RoleSeedModel> roleSeedModels)
        {
            List<RoleEntity> existingRoles = _context.Roles.ToList();

            IEnumerable<RoleEntity> missingRoles = roleSeedModels
                .Where(x => !existingRoles.Any(c => c.NormalizedName == x.Name.ToUpper()))
                .Select(x => x.ToEntity());

            foreach(RoleEntity roleEntity in missingRoles)
            {
                IdentityResult createRoleResult = await _roleManager.CreateAsync(roleEntity);
                if (!createRoleResult.Succeeded)
                {
                    _logger.LogCritical($"Failed to seed Role. Name {roleEntity.Name}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                    throw new Exception($"Failed to seed Role.");
                }
            }
        }

        public void SeedRolePermissions(List<RoleSeedModel> roleSeedModels)
        {
            List<RoleEntity> roles = _context.Roles
                .Include(x => x.Permissions)
                .ThenInclude(x => x.Permission)
                .ToList();

            List<PermissionEntity> permissions = _context.Permissions.ToList();

            List<PermissionRoleEntity> permissionRoleEntities = new List<PermissionRoleEntity>();

            foreach(RoleSeedModel role in roleSeedModels)
            {
                RoleEntity roleEntity = roles
                    .Where(x => x.NormalizedName == role.Name.ToUpper())
                    .SingleOrDefault();

                if(roleEntity == null)
                {
                    _logger.LogCritical($"No role Role. Name {role.Name}");
                    throw new Exception($"No Role.");
                }

                foreach (string permission in role.Permissions)
                {
                    bool exists = roleEntity.Permissions
                        .Where(x => x.Permission.Name.ToUpper() == permission.ToUpper())
                        .Any();
                    if (exists)
                    {
                        continue;
                    }

                    PermissionEntity permissionEntity = permissions
                        .Where(x => x.Name.ToUpper() == permission.ToUpper())
                        .SingleOrDefault();
                    if (permissionEntity == null)
                    {
                        _logger.LogCritical($"Missing role permission. Role: {role.Name}, Permission {permission}");
                        throw new Exception("Missing role permission");
                    }

                    PermissionRoleEntity permissionRoleEntity = new PermissionRoleEntity(
                        permissionEntity.Id,
                        roleEntity.Id);

                    permissionRoleEntities.Add(permissionRoleEntity);
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

        public void SeedRoleAssigments(List<RoleSeedModel> roleSeedModels)
        {
            List<RoleEntity> roles = _context.Roles
                .Include(x => x.CanAssigne)
                .ThenInclude(x => x.Role)
                .ToList();

            List<RoleAssignmentEntity> roleAssignmentEntities = new List<RoleAssignmentEntity>();

            foreach(RoleSeedModel role in roleSeedModels)
            {
                if(!role.RoleAssignments.Any())
                {
                    continue;
                }

                RoleEntity roleEntity = roles
                    .Where(x => x.NormalizedName == role.Name.ToUpper())
                    .SingleOrDefault();

                if (roleEntity == null)
                {
                    _logger.LogCritical($"No role Role. Name {roleEntity.Name}");
                    throw new Exception($"No Role.");
                }

                foreach(string assigment in role.RoleAssignments)
                {
                    bool exists = roles
                        .Where(x => x.CanAssigne.Any(c => c.CanAssigneRole.NormalizedName == assigment.ToUpper()))
                        .Any();
                    if(exists)
                    {
                        continue;
                    }

                    RoleEntity assigneRoleEntity = roles
                        .Where(x => x.NormalizedName == assigment.ToUpper())
                        .SingleOrDefault();

                    if (assigneRoleEntity == null)
                    {
                        _logger.LogCritical($"No assign role. Name {assigneRoleEntity.Name}");
                        throw new Exception($"No assign Role.");
                    }

                    if (assigneRoleEntity.NormalizedName == roleEntity.NormalizedName)
                    {
                        _logger.LogCritical($"Role can not assign self. Role name {roleEntity.Name}");
                        throw new Exception($"Role can not assign self");
                    }

                    if(assigneRoleEntity.Type != RoleTypes.Group)
                    {
                        _logger.LogCritical($"Wrong role type for role assignment. Role name {roleEntity.Name}");
                        throw new Exception($"Wrong role type for role assignment");
                    }

                    RoleAssignmentEntity roleAssignmentEntity = new RoleAssignmentEntity(
                        roleEntity.Id,
                        assigneRoleEntity.Id);

                    roleAssignmentEntities.Add(roleAssignmentEntity);
                }
            }

            _context.RoleAssignments.AddRange(roleAssignmentEntities);
            int roleAssignmentsChanges = _context.SaveChanges();
            if (roleAssignmentsChanges != roleAssignmentEntities.Count())
            {
                _logger.LogCritical($"Failed to seed role assignments");
                throw new Exception("Failed to seed role assignments");
            }
        }

        private void SeedMissingEmails()
        {
            List<EmailTypes> emailTypes = _context.Emails
                .Select(x => x.Type)
                .ToList();

            IEnumerable<EmailEntity> missingEmails = _allEmails
                .Where(x => !emailTypes.Contains(x.Type));

            _context.Emails.AddRange(missingEmails);
            int changes = _context.SaveChanges();
            if (changes != missingEmails.Count())
            {
                _logger.LogCritical($"Failed to seed emails");
                throw new Exception("Failed to seed emails");
            }
        }
    }
}
