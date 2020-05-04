using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Models;

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    public static class IdentityDbContextSeeder
    {
        /// <summary>
        /// Seeds identityAdmin
        /// </summary>
        /// <param name="app"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SeedIdentityAdmin(this IApplicationBuilder app, string userName, string password)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                IdentityDbContext context = serviceProvider.GetService<IdentityDbContext>();
                UserManager<AppUserEntity> userManager = serviceProvider.GetService<UserManager<AppUserEntity>>();
                RoleManager<RoleEntity> roleManager = serviceProvider.GetService<RoleManager<RoleEntity>>();

                ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(IdentityDbContextSeeder));

                if (context.Users.Any())
                {
                    logger.LogInformation($"Admin was not seeded, because Users table is not empty");
                    return;
                }

                SeedIdentityAdmin(userName, password, context, userManager, logger);
            }
        }

        /// <summary>
        /// Seeds admin
        /// </summary>
        /// <param name="app"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        public static void SeedIdentityAdmin(this IApplicationBuilder app, string userName, string password, List<string> roles)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                IdentityDbContext context = serviceProvider.GetService<IdentityDbContext>();
                UserManager<AppUserEntity> userManager = serviceProvider.GetService<UserManager<AppUserEntity>>();

                ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(IdentityDbContextSeeder));

                if (context.Users.Any())
                {
                    logger.LogInformation($"Admin was not seeded, because Users table is not empty");
                    return;
                }

                Task.WaitAll(SeedAdmin(userName, password, roles, userManager, logger));
            }
        }

        /// <summary>
        /// Creates a new database without migrations
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder CreateIdentityDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IdentityDbContext context = scope.ServiceProvider.GetService<IdentityDbContext>();

                ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(IdentityDbContextSeeder));

                bool result = context.Database.EnsureCreated();
                if (result)
                {
                    logger.LogInformation($"IdentityDatabase created");
                }
            }

            return app;
        }

        /// <summary>
        /// First deletes old database and than creates a new one without migrations
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder ResetIdentityDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IdentityDbContext context = scope.ServiceProvider.GetService<IdentityDbContext>();
                
                ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(IdentityDbContextSeeder));

                bool deleteResult = context.Database.EnsureDeleted();
                if (deleteResult)
                {
                    logger.LogInformation($"IdentityDatabase deleted");
                }

                bool createdResult = context.Database.EnsureCreated();
                if (createdResult)
                {
                    logger.LogInformation($"IdentityDatabase created");
                }
            }

            return app;
        }

        /// <summary>
        /// Seeds admin and the following users
        /// <br>carson.alexander@{your email domain}</br>
        /// <br>merdith.alonso@{your email domain}</br>
        /// <br>arturo.anad@{your email domain}</br>
        /// <br>gytis.barzdukas@{your email domain}</br>
        /// <br>yan.li@{your email domain}</br>
        /// <br>peggy.justice@{your email domain}</br>
        /// <br>laura.norman@{your email domain}</br>
        /// <br>nino.olivetto@{your email domain}</br>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="password"></param>
        /// <param name="emailDomain"></param>
        /// <param name="adminUserName"></param>
        /// <param name="adminPassword"></param>
        public static void SeedDatabase(this IApplicationBuilder app, string password = "Password1!", string emailDomain = "ssrd.io",
            string adminUserName = "identityAdmin", string adminPassword = "Password11!")
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                IdentityDbContext context = serviceProvider.GetService<IdentityDbContext>();
                UserManager<AppUserEntity> userManager = serviceProvider.GetService<UserManager<AppUserEntity>>();
                RoleManager<RoleEntity> roleManager = serviceProvider.GetService<RoleManager<RoleEntity>>();

                ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(IdentityDbContextSeeder));

                if (context.Users.Any())
                {
                    logger.LogInformation($"Database was not seeded, because Users table is not empty");
                    return;
                }

                SeedIdentityAdmin(adminUserName, adminPassword, context, userManager, logger);
                Seed(emailDomain, password, userManager, roleManager, logger);
            }
        }

        private static void SeedIdentityAdmin(string userName, string password, IdentityDbContext context,
            UserManager<AppUserEntity> userManager, ILogger logger)
        {
            AppUserEntity admin = new AppUserEntity()
            {
                UserName = userName,
                EmailConfirmed = true,
                Enabled = true,
            };

            {
                Task<IdentityResult> createAdmin = userManager.CreateAsync(admin, password);
                Task.WaitAll(createAdmin);

                IdentityResult createAdminResult = createAdmin.Result;
                if (!createAdminResult.Succeeded)
                {
                    throw new Exception($"Failed to add identity admin. {string.Join(" ", createAdminResult.Errors.Select(x => x.Description))}");
                }
            }

            RoleEntity role = context.Roles
                .Where(x => x.Name == IdentityUIRoles.IDENTITY_MANAGMENT_ROLE)
                .SingleOrDefault();

            if (role == null)
            {
                logger.LogCritical($"No IdentityUI admin role");
                throw new Exception("No IdentityUI admin role");
            }

            UserRoleEntity userRole = new UserRoleEntity(admin.Id, role.Id);

            context.UserRoles.Add(userRole);

            int changes = context.SaveChanges();
            if (changes <= 0)
            {
                throw new Exception($"Failed to link admin to admin role");
            }

            logger.LogInformation($"IdentityAdmin was seeded");
        }

        private static async Task SeedAdmin(string userName, string password, List<string> adminRoles, UserManager<AppUserEntity> userManager, ILogger logger)
        {
            AppUserEntity appUser = await userManager.FindByNameAsync(userName);
            if(appUser == null)
            {
                appUser = new AppUserEntity()
                {
                    UserName = userName,
                    EmailConfirmed = true,
                    Enabled = true,
                };

                IdentityResult createAdminResult = await userManager.CreateAsync(appUser, password);

                if (!createAdminResult.Succeeded)
                {
                    throw new Exception($"Failed to add admin. {string.Join(" ", createAdminResult.Errors.Select(x => x.Description))}");
                }
            }

            IdentityResult addToRolesResult = await userManager.AddToRolesAsync(appUser, adminRoles); //chack if any roles are group roles
            if(!addToRolesResult.Succeeded)
            {
                throw new Exception($"Failed to add admin to roles. {string.Join(" ", addToRolesResult.Errors.Select(x => x.Description))}");
            }

            logger.LogInformation("Admin was added");
        }

        private static void Seed(string emailDomain, string password, UserManager<AppUserEntity> userManager, RoleManager<RoleEntity> roleManager, ILogger logger)
        {
            AppUserEntity[] sampleUsers = new AppUserEntity[]
            {
                    new AppUserEntity{FirstName="Carson",LastName="Alexander",UserName=$"carson.alexander@{emailDomain}", Email=$"carson.alexander@{emailDomain}", EmailConfirmed=true, Enabled=true},
                    new AppUserEntity{FirstName="Meredith",LastName="Alonso",UserName=$"merdith.alonso@{emailDomain}", Email=$"merdith.alonso@{emailDomain}", EmailConfirmed=true, Enabled=true},
                    new AppUserEntity{FirstName="Arturo",LastName="Anand",UserName=$"arturo.anad@{emailDomain}", Email=$"arturo.anad@{emailDomain}", EmailConfirmed=true, Enabled=true},
                    new AppUserEntity{FirstName="Gytis",LastName="Barzdukas",UserName=$"gytis.barzdukas@{emailDomain}", Email=$"gytis.barzdukas@{emailDomain}", EmailConfirmed=true},
                    new AppUserEntity{FirstName="Yan",LastName="Li",UserName=$"yan.li@{emailDomain}", Email=$"yan.li@{emailDomain}", Enabled=true},
                    new AppUserEntity{FirstName="Peggy",LastName="Justice",UserName=$"peggy.justice@{emailDomain}", Email=$"peggy.justice@{emailDomain}"},
                    new AppUserEntity{FirstName="Laura",LastName="Norman",UserName=$"laura.norman@{emailDomain}", Email=$"laura.norman@{emailDomain}", EmailConfirmed=true, Enabled=true},
                    new AppUserEntity{FirstName="Nino",LastName="Olivetto",UserName=$"nino.olivetto@{emailDomain}", Email=$"nino.olivetto@{emailDomain}"}
            };

            foreach (var user in sampleUsers)
            {
                Task.WaitAll(userManager.CreateAsync(user, password));
            }

            Task.WaitAll(roleManager.CreateAsync(new RoleEntity("Admin", "Admin role", RoleTypes.System)));
            Task.WaitAll(roleManager.CreateAsync(new RoleEntity("Normal", "Normal role", RoleTypes.System)));

            logger.LogInformation($"Identity database was Seeded");
        }

        public static void SeedIdentityUIEntities(this IApplicationBuilder app, List<RoleData> roles)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;

                IdentityDbContext context = serviceProvider.GetService<IdentityDbContext>();
                UserManager<AppUserEntity> userManager = serviceProvider.GetService<UserManager<AppUserEntity>>();
                RoleManager<RoleEntity> roleManager = serviceProvider.GetService<RoleManager<RoleEntity>>();

                ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(IdentityDbContextSeeder));

                if (context.Roles.Any() || context.Permissions.Any() || context.PermissionRole.Any())
                {
                    logger.LogInformation($"Database is not empty. Skipping seeded system entities");
                    return;
                }

                if (context.Emails.Any())
                {
                    logger.LogInformation($"Database is not empty. Skipping seeded system entities");
                    return;
                }

                SeedIdentityUIRolesPermissions(context, roleManager, logger);
                SeedIdentityUIEmails(context, logger);

                if (roles != null)
                {
                    SeedRolesAndPermissions(context, roleManager, roles, logger);
                }
            }
        }

        private static void SeedIdentityUIRolesPermissions(IdentityDbContext context, RoleManager<RoleEntity> roleManager, ILogger logger)
        {
            List<PermissionEntity> permissions = new List<PermissionEntity>();

            foreach (string permission in IdentityUIPermissions.ALL_PERMISSIONS)
            {
                PermissionEntity permissionEntity = new PermissionEntity(
                    name: permission,
                    description: null);

                permissions.Add(permissionEntity);
            }

            context.Permissions.AddRange(permissions);
            int addPermissionChanges = context.SaveChanges();
            if (addPermissionChanges != permissions.Count)
            {
                logger.LogCritical($"Failed to add system Permission.");
                throw new Exception("failed_to_add_system_permission");
            }

            List<RoleEntity> roles = new List<RoleEntity>();
            foreach (string roleName in IdentityUIRoles.ALL_ROLES)
            {
                RoleEntity roleEntity = new RoleEntity(roleName, null, RoleTypes.System);

                Task<IdentityResult> createRole = roleManager.CreateAsync(roleEntity);
                Task.WaitAll(createRole);

                IdentityResult createRoleResult = createRole.Result;
                if (!createRoleResult.Succeeded)
                {
                    logger.LogCritical($"Failed to add IdentityUI system role. RoleName {roleName}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                    throw new Exception($"Failed to add IdentityUI system role.");
                }

                roles.Add(roleEntity);
            }

            RoleEntity identityAdminRole = context.Roles
                .Where(x => x.Name == IdentityUIRoles.IDENTITY_MANAGMENT_ROLE)
                .SingleOrDefault();

            if (identityAdminRole == null)
            {
                logger.LogCritical($"No IdentityUI admin role");
                throw new Exception("No IdentityUI admin role");
            }

            IEnumerable<PermissionRoleEntity> permissionRoles = permissions
                .Select(x => new PermissionRoleEntity(
                    permissionId: x.Id,
                    roleId: identityAdminRole.Id));

            context.PermissionRole.AddRange(permissionRoles);
            int addPermissionRoleChanges = context.SaveChanges();
            if (addPermissionRoleChanges != permissionRoles.Count())
            {
                logger.LogCritical($"Failed to add IdentityUI admin role permissions");
                throw new Exception("Failed to add IdentityUI admin role permissions");
            }
        }

        private static void SeedRolesAndPermissions(IdentityDbContext context, RoleManager<RoleEntity> roleManager, List<RoleData> roles, ILogger logger)
        {
            List<string> permissionNames = roles
                .SelectMany(x => x.Permissions)
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            List<PermissionEntity> permissions = context.Permissions
                .Where(x => permissionNames.Select(c => c.ToUpper()).Contains(x.Name.ToUpper()))
                .ToList();

            List<string> missingPermissions = permissionNames
                .Where(x => !permissions.Select(c => c.Name.ToUpper()).Contains(x.ToUpper()))
                .ToList();

            foreach (string permission in missingPermissions)
            {
                PermissionEntity permissionEntity = new PermissionEntity(
                    name: permission,
                    description: null);

                permissions.Add(permissionEntity);
            }

            context.Permissions.AddRange(permissions);
            int addPermissionChanges = context.SaveChanges();
            if (addPermissionChanges != permissions.Count)
            {
                logger.LogCritical($"Failed to add Permission.");
                throw new Exception("failed_to_add_permission");
            }

            List<PermissionRoleEntity> permissionRoles = new List<PermissionRoleEntity>();

            foreach (RoleData roleData in roles)
            {
                RoleEntity roleEntity = new RoleEntity(roleData.Name, roleData.Description, RoleTypes.System);

                Task<IdentityResult> createRole = roleManager.CreateAsync(roleEntity);
                Task.WaitAll(createRole);

                IdentityResult createRoleResult = createRole.Result;
                if (!createRoleResult.Succeeded)
                {
                    logger.LogCritical($"Failed to add role. RoleName {roleData.Name}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                    throw new Exception($"Failed to add role.");
                }

                foreach(PermissionData permissionData in roleData.Permissions)
                {
                    PermissionEntity permission = permissions
                        .Where(x => x.Name.ToUpper() == permissionData.Name.ToUpper())
                        .Single();

                    PermissionRoleEntity permissionRoleEntity = new PermissionRoleEntity(
                        permissionId: permission.Id,
                        roleId: roleEntity.Id);

                    permissionRoles.Add(permissionRoleEntity);
                }
            }

            context.PermissionRole.AddRange(permissionRoles);
            int addPermissionRoleChanges = context.SaveChanges();
            if (addPermissionRoleChanges != permissionRoles.Count)
            {
                logger.LogCritical($"Failed to add Permission Roles.");
                throw new Exception("failed_to_add_permission_roles");
            }
        }

        private static void SeedIdentityUIEmails(IdentityDbContext context, ILogger logger)
        {
            List<EmailEntity> emails = new List<EmailEntity>();

            EmailEntity inviteEmail = new EmailEntity(
                subject: "Invitation",
                body: "You have been invited. If you wish to register <a href='{{token}}'>click here</a>",
                type: EmailTypes.Invite);
            emails.Add(inviteEmail);

            EmailEntity confirmationEmail = new EmailEntity(
                subject: "Confirm your email",
                body: "Please confirm your account by <a href='{{token}}'>clicking here</a>.",
                type: EmailTypes.EmailConfirmation);
            emails.Add(confirmationEmail);

            EmailEntity passwordRecovery = new EmailEntity(
                subject: "Reset Password",
                body: "Please reset your password by <a href='{{token}}'>clicking here</a>",
                type: EmailTypes.PasswordRecovery);
            emails.Add(passwordRecovery);

            EmailEntity passwordWasReset = new EmailEntity(
                subject: "Password was reset",
                body: "Your password has been reset",
                type: EmailTypes.PasswordWasReset);
            emails.Add(passwordWasReset);

            context.Emails.AddRange(emails);
            int changes = context.SaveChanges();
            if(changes != emails.Count)
            {
                logger.LogCritical($"Failed to add system emails");
                throw new Exception("Failed to add system emails");
            }
        }
    }
}
