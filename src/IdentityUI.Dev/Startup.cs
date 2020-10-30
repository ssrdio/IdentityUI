using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SSRD.Audit.Filters;
using SSRD.IdentityUI.Account;
using SSRD.IdentityUI.Admin;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin;
using SSRD.IdentityUI.Core;
using SSRD.IdentityUI.EntityFrameworkCore.Postgre.DependencyInjection;
using SSRD.IdentityUI.EntityFrameworkCore.SqlServer.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

                    endpoints.ShowAuditToUser = false;
                })
                .UsePostgre()
                //.UseSqlServer()
                //.UseInMemoryDatabase()
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

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new AuditAttribute());
            });

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

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityUI API", Version = "v1.0.0" });

                // Set the comments path for the Swagger JSON and UI.
                //string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //options.IncludeXmlComments(xmlPath);

                //IEnumerable<string> xmlFiles = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                //    .Select(x => Path.Combine(AppContext.BaseDirectory, $"{x.Name}.xml"))
                //    .Where(x => File.Exists(x));

                //foreach (string xmlDocFile in xmlFiles)
                //{
                //    options.IncludeXmlComments(xmlDocFile);
                //}
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityUI API");
                });
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
                endpoints.MapGroupAdmin();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
