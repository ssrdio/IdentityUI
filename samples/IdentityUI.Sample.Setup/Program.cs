using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;

namespace IdentityUI.Sample.Setup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLog.Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                IHost host = CreateHostBuilder(args).Build();

                PrepereDatabase(host, logger);

                logger.Debug("run host");
                host.Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
              })
              .ConfigureLogging(logging =>
              {
                  logging.SetMinimumLevel(LogLevel.Trace);
              })
              .UseNLog();

        public static void PrepereDatabase(IHost host, NLog.ILogger logger)
        {
            logger.Info($"Preparing IdentityUI database");

            using IServiceScope serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

            serviceProvider.RunIdentityMigrations();
            serviceProvider.SeedDatabase(adminUserName: configuration["IdentityUI:Admin:Username"], adminPassword: configuration["IdentityUI:Admin:Password"]);
        }
    }
}
