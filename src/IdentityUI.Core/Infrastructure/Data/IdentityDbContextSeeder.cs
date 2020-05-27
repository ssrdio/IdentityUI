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
using SSRD.IdentityUI.Core.Infrastructure.Data.Seeders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    public static class IdentityDbContextSeeder
    {
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
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedDatabase(password, emailDomain, adminUserName, adminPassword);
            }
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
        /// <param name="host"></param>
        /// <param name="password"></param>
        /// <param name="emailDomain"></param>
        /// <param name="adminUserName"></param>
        /// <param name="adminPassword"></param>
        public static void SeedDatabase(this IHost host, string password = "Password1!", string emailDomain = "ssrd.io",
            string adminUserName = "identityAdmin", string adminPassword = "Password11!")
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedDatabase(password, emailDomain, adminUserName, adminPassword);
            }
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
        /// <param name="webHost"></param>
        /// <param name="password"></param>
        /// <param name="emailDomain"></param>
        /// <param name="adminUserName"></param>
        /// <param name="adminPassword"></param>
        public static void SeedDatabase(this IWebHost webHost, string password = "Password1!", string emailDomain = "ssrd.io",
            string adminUserName = "identityAdmin", string adminPassword = "Password11!")
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedDatabase(password, emailDomain, adminUserName, adminPassword);
            }
        }

        public static void SeedDatabase(this IServiceProvider serviceProvider, string password = "Password1!", string emailDomain = "ssrd.io",
            string adminUserName = "identityAdmin", string adminPassword = "Password11!")
        {
            SystemEntitySeeder systemEntitySeeder = serviceProvider.GetRequiredService<SystemEntitySeeder>();
            AdminSeeder adminSeeder = serviceProvider.GetRequiredService<AdminSeeder>();
            UserSeeder userSeeder = serviceProvider.GetRequiredService<UserSeeder>();

            Task.WaitAll(systemEntitySeeder.SeedIdentityUI());
            Task.WaitAll(adminSeeder.SeedIdentityAdmin(adminUserName, adminPassword));
            Task.WaitAll(userSeeder.Seed(emailDomain, password));
        }

        /// <summary>
        /// Seeds identityAdmin
        /// </summary>
        /// <param name="app"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SeedIdentityAdmin(this IApplicationBuilder app, string userName, string password)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedIdentityAdmin(userName, password);
            }
        }

        /// <summary>
        /// Seeds identityAdmin
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SeedIdentityAdmin(this IHost host, string userName, string password)
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedIdentityAdmin(userName, password);
            }
        }

        /// <summary>
        /// Seeds identityAdmin
        /// </summary>
        /// <param name="webHost"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SeedIdentityAdmin(this IWebHost webHost, string userName, string password)
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedIdentityAdmin(userName, password);
            }
        }

        /// <summary>
        /// Seeds identityAdmin
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SeedIdentityAdmin(this IServiceProvider serviceProvider, string userName, string password)
        {
            AdminSeeder adminSeeder = serviceProvider.GetRequiredService<AdminSeeder>();

            Task.WaitAll(adminSeeder.SeedIdentityAdmin(userName, password));
        }

        /// <summary>
        /// Seeds admin
        /// </summary>
        /// <param name="app"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        public static void SeedAdmin(this IApplicationBuilder app, string userName, string password, List<string> roles)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedAdmin(userName, password, roles);
            }
        }

        /// <summary>
        /// Seeds admin
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        public static void SeedAdmin(this IHost host, string userName, string password, List<string> roles)
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedAdmin(userName, password, roles);
            }
        }

        /// <summary>
        /// Seeds admin
        /// </summary>
        /// <param name="webHost"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        public static void SeedAdmin(this IWebHost webHost, string userName, string password, List<string> roles)
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedAdmin(userName, password, roles);
            }
        }

        /// <summary>
        /// Seeds admin
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        public static void SeedAdmin(this IServiceProvider serviceProvider, string userName, string password, List<string> roles)
        {
            AdminSeeder adminSeeder = serviceProvider.GetRequiredService<AdminSeeder>();

            Task.WaitAll(adminSeeder.Seed(userName, password, roles));
        }

        /// <summary>
        /// Seed system entities 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedSystemEntities(this IApplicationBuilder app, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedSystemEntities(permissionSeedModels, roleSeedModels);
            }
        }

        /// <summary>
        /// Seed system entities 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedSystemEntities(this IHost host, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedSystemEntities(permissionSeedModels, roleSeedModels);
            }
        }

        /// <summary>
        /// Seed system entities 
        /// </summary>
        /// <param name="webHost"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedSystemEntities(this IWebHost webHost, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedSystemEntities(permissionSeedModels, roleSeedModels);
            }
        }

        /// <summary>
        /// Seed system entities 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedSystemEntities(this IServiceProvider serviceProvider, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            if(permissionSeedModels == null)
            {
                permissionSeedModels = new List<PermissionSeedModel>();
            }

            if(roleSeedModels == null)
            {
                roleSeedModels = new List<RoleSeedModel>();
            }

            SystemEntitySeeder systemEntitySeeder = serviceProvider.GetRequiredService<SystemEntitySeeder>();

            Task.WaitAll(systemEntitySeeder.Seed(permissionSeedModels, roleSeedModels));
        }

        /// <summary>
        /// Seed missing system entities 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedMissingSystemEntities(this IApplicationBuilder app, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedMissingSystemEntities(permissionSeedModels, roleSeedModels);
            }
        }

        /// <summary>
        /// Seed missing system entities 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedMissngSystemEntities(this IHost host, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedMissingSystemEntities(permissionSeedModels, roleSeedModels);
            }
        }

        /// <summary>
        /// Seed missing system entities 
        /// </summary>
        /// <param name="webHost"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedMissngSystemEntities(this IWebHost webHost, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.SeedMissingSystemEntities(permissionSeedModels, roleSeedModels);
            }
        }

        /// <summary>
        /// Seed missing system entities 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="permissionSeedModels">Permissions required for your application</param>
        /// <param name="roleSeedModels">Roles required for your application.</param>
        public static void SeedMissingSystemEntities(this IServiceProvider serviceProvider, List<PermissionSeedModel> permissionSeedModels = null,
            List<RoleSeedModel> roleSeedModels = null)
        {
            if (permissionSeedModels == null)
            {
                permissionSeedModels = new List<PermissionSeedModel>();
            }

            if (roleSeedModels == null)
            {
                roleSeedModels = new List<RoleSeedModel>();
            }

            SystemEntitySeeder systemEntitySeeder = serviceProvider.GetRequiredService<SystemEntitySeeder>();

            Task.WaitAll(systemEntitySeeder.SeedMissing(permissionSeedModels, roleSeedModels));
        }
    }
}
