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

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    public static class IdentityDbContextSeeder
    {
        /// <summary>
        /// Seeds admin
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

                if (context.Roles.Any())
                {
                    logger.LogInformation($"Admin was no seeded, because Role tabe is not empty");
                    return;
                }

                SeedAdmin(userName, password, context, userManager, roleManager, logger);
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

                if (context.Roles.Any())
                {
                    logger.LogInformation($"Database was no seeded, because Role table is not empty");
                    return;
                }

                SeedSystemEntities(context, roleManager, logger);

                SeedAdmin(adminUserName, adminPassword, context, userManager, roleManager, logger);
                Seed(emailDomain, password, userManager, roleManager, logger);
            }
        }

        private static void SeedAdmin(string userName, string password, IdentityDbContext context, UserManager<AppUserEntity> userManager,
            RoleManager<RoleEntity> roleManager, ILogger logger)
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
                    throw new Exception($"Failed to add admin. {string.Join(" ", createAdminResult.Errors.Select(x => x.Description))}");
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

            logger.LogInformation($"Admin was seeded");
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

        private static void SeedSystemEntities(IdentityDbContext context, RoleManager<RoleEntity> roleManager, ILogger logger)
        {
            if(context.Roles.Any() || context.Permissions.Any() || context.PermissionRole.Any())
            {
                logger.LogInformation($"Databse is not empty. Skiping seeded system entities");
                return;
            }

            List<PermissionEntity> permissions = new List<PermissionEntity>();

            foreach(string permission in IdentityUIPermissions.ALL_PERMISSIONS)
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
            foreach(string roleName in IdentityUIRoles.ALL_ROLES)
            {
                RoleEntity roleEntity = new RoleEntity(roleName, null, RoleTypes.System);
                roles.Add(roleEntity);

                Task<IdentityResult> createRole = roleManager.CreateAsync(roleEntity);
                Task.WaitAll(createRole);

                IdentityResult createRoleResult = createRole.Result;
                if (!createRoleResult.Succeeded)
                {
                    logger.LogCritical($"Failed to add IdentityUI system role. RoleNam {roleName}, Error {string.Join(" ", createRoleResult.Errors.Select(x => x.Description))}");
                    throw new Exception($"Failed to add IdentityUI system role.");
                }
            }

            RoleEntity identityAdminRole = context.Roles
                .Where(x => x.Name == IdentityUIRoles.IDENTITY_MANAGMENT_ROLE)
                .SingleOrDefault();

            if(identityAdminRole == null)
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
            if(addPermissionRoleChanges != permissionRoles.Count())
            {
                logger.LogCritical($"Failed to add IdentityUI admin role permissions");
                throw new Exception("Failed to add IdentityUI admin role permissions");
            }

            SeedEmails(context, logger);
        }

        private static void SeedEmails(IdentityDbContext context, ILogger logger)
        {
            if(context.Emails.Any())
            {
                logger.LogInformation($"Database is not empty. Skiping seeded system entities");
                return;
            }

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
