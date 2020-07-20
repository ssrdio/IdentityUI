using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using System;

namespace IdentityUI.Sample.Sms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                IHost host = CreateHostBuilder(args).Build();

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
                  logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
              })
              .UseNLog();
    }
}
