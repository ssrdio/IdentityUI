using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment
{
    public static class ReleaseManagementExtensions
    {
        /// <summary>
        /// Runs all missing migrations on the database
        /// </summary>
        /// <param name="app"></param>
        public static void RunIdentityMigrations(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.RunIdentityMigrations();
            }
        }

        /// <summary>
        /// Runs all missing migrations on the database
        /// </summary>
        /// <param name="host"></param>
        public static void RunIdentityMigrations(this IHost host)
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.RunIdentityMigrations();
            }
        }

        /// <summary>
        /// Runs all missing migrations on the database
        /// </summary>
        /// <param name="webHost"></param>
        public static void RunIdentityMigrations(this IWebHost webHost)
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.RunIdentityMigrations();
            }
        }

        /// <summary>
        /// Runs all missing migrations on the database
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void RunIdentityMigrations(this IServiceProvider serviceProvider)
        {
            IReleaseManagement releaseManagement = serviceProvider.GetRequiredService<IReleaseManagement>();

            releaseManagement.ApplayMigrations();
        }

        /// <summary>
        /// Creates a new database without migrations
        /// </summary>
        /// <param name="app"></param>
        public static void CreateIdentityDatabase(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.CreateIdentityDatabase();
            }
        }

        /// <summary>
        /// Creates a new database without migrations
        /// </summary>
        /// <param name="host"></param>
        public static void CreateIdentityDatabase(this IHost host)
        {
            using (IServiceScope scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.CreateIdentityDatabase();
            }
        }

        /// <summary>
        /// Creates a new database without migrations
        /// </summary>
        /// <param name="webHost"></param>
        public static void CreateIdentityDatabase(this IWebHost webHost)
        {
            using (IServiceScope scope = webHost.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.CreateIdentityDatabase();
            }
        }

        /// <summary>
        /// Creates a new database without migrations
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void CreateIdentityDatabase(this IServiceProvider serviceProvider)
        {
            IdentityDbContext context = serviceProvider.GetService<IdentityDbContext>();

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger(typeof(ReleaseManagementExtensions));

            bool result = context.Database.EnsureCreated();
            if (result)
            {
                logger.LogInformation($"IdentityDatabase created");
            }
        }
    }
}
