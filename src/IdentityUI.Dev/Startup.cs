using IdentityUI.Dev.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSRD.IdentityUI.Account;
using SSRD.IdentityUI.Admin;
using SSRD.IdentityUI.Core;
using System;

namespace IdentityUI.Dev
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
            .AddAccountManagement()
            .AddEmailSender();

            services.AddScoped<UserSeeder>();

            services.AddControllersWithViews();

#if DEBUG
            services.AddControllersWithViews().AddRazorRuntimeCompilation(o =>
            {

                string basePath = AppContext.BaseDirectory;
                string accountPath = System.IO.Path.Combine(basePath, "../../../../IdentityUI.Account");
                string adminPath = System.IO.Path.Combine(basePath, "../../../../IdentityUI.Admin");

                o.FileProviders.Add(new Microsoft.Extensions.FileProviders.PhysicalFileProvider(accountPath));
                o.FileProviders.Add(new Microsoft.Extensions.FileProviders.PhysicalFileProvider(adminPath));
            });
#endif

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
