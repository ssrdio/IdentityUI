using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSRD.IdentityUI.Account;
using SSRD.IdentityUI.Admin;
using SSRD.IdentityUI.Core;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.RevisionLogger.Extensions;

namespace IdentityUI.Sample.Setup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureIdentityUI(Configuration, endpoints =>
            {
                endpoints.Home = "/";

                endpoints.Login = "/Account/Login/";
                endpoints.Logout = "/Account/Logout/";
                endpoints.AccessDenied = "/Account/AccessDenied/";

                endpoints.Manage = "/Account/Manage/";
                endpoints.ConfirmeEmail = "/Account/ConfirmEmail";
                endpoints.ResetPassword = "/Account/ResetPassword";

                endpoints.RegisterEnabled = true;

                endpoints.UseEmailSender = true;
                endpoints.UseSmsGateway = true;
            })
            .UseInMemoryDatabase()
            .AddIdentityUI(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddAuth(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Account/Login/";
                options.AccessDeniedPath = "/Account/AccessDenied/";
                options.SlidingExpiration = true;
                options.LogoutPath = "/Account/Logout/";
            })
            .AddIdentityAdmin()
            .AddAccountManagement();

            services.ConfigureRevisionLogger(options =>
            {
                options.LogQueryData = false;
                options.LogBodyData = false;

                options.LogBodyDataMaxSize = 30 * 1024;
                options.IgnoreLogignBodyForRoutes = new List<string>
                {
                    "Account/.*",
                    "IdentityAdmin/User/SetNewPassword",
                };

                options.UserIdentityClaimType = ClaimTypes.NameIdentifier;
                options.UseXForwardedForIp = true;

                options.Version = Assembly.GetExecutingAssembly().GetName()?.Version?.ToString();
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityUI();

            app.UseRevisionLogger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAccountManagement();
                endpoints.MapIdentityAdmin();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
