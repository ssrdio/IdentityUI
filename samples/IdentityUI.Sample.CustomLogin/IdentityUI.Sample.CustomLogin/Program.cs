using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;

namespace IdentityUI.CustomLogin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                IHost host = CreateHostBuilder(args).Build();

                host.CreateIdentityDatabase();
                host.SeedSystemEntities();

                host.SeedIdentityAdmin("admin", "Password");
                host.Run();
            }
            catch (Exception)
            {
                throw;
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
              });
    }
}
