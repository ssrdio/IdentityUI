using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Core.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SSRD.AdminUI.Template;
using SSRD.IdentityUI.Account.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using System;
using SSRD.IdentityUI.Account.Areas.Account.Services;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using FluentValidation;
using SSRD.IdentityUI.Account.Areas.Account.Models.Audit;

namespace SSRD.IdentityUI.Account
{
    public static class AccountManagementExtension
    {
        /// <summary>
        /// Registers services for IdentityUI.Account
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddAccountManagement(this IdentityUIServicesBuilder builder)
        {
            PagePath.HOME = builder.IdentityManagementEndpoints.Home;

            builder.Services.AddAdminTemplate();

            builder.Services.AddScoped<IManageDataService, ManageDataService>();
            builder.Services.AddScoped<IAccountDataService, AccountDataService>();
            builder.Services.AddScoped<ITwoFactorAuthenticationDataService, TwoFactorAuthenticationDataService>();
            builder.Services.AddScoped<ICredentialsDataService, CredentialsDataService>();

            builder.Services.AddScoped<IAuditDataService, AuditDataService>();

            builder.Services.AddSingleton<IValidator<AuditTableRequest>, AuditTableRequestValidator>();

            builder.Services.ConfigureOptions(typeof(UIConfigureOptions));

            return builder;
        }

        [Obsolete("UseAccountManagement is obsolete use MapAccountManagement instead")]
        /// <summary>
        /// Adds IdentityUI.Account to the specified Microsoft.AspNetCore.Builder.IApplicationBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIAppBuilder UseAccountManagement(this IdentityUIAppBuilder builder)
        {
#if NET_CORE2
            builder.App.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "Account",
                    areaName: "Account",
                    template: "Account/{controller=Home}/{action=Index}/{id?}");
            });
#endif
#if NET_CORE3
            builder.App.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    "areas",
                    "Account",
                    "Account/{controller=Home}/{action=Index}/{id?}");
            }); 
#endif
            return builder;
        }

#if NET_CORE2
        public static void MapAccountManagement(this IRouteBuilder routes)
        {
            routes.MapAreaRoute(
                name: "Account",
                areaName: "Account",
                template: "Account/{controller=Home}/{action=Index}/{id?}");
        }
#elif NET_CORE3
        public static void MapAccountManagement(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapAreaControllerRoute(
                "areas",
                "Account",
                "Account/{controller=Home}/{action=Index}/{id?}");
        }
#endif
    }
}
