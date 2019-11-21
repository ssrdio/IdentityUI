using FluentValidation;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Role;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.User;
using SSRD.IdentityUI.Core.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Admin.DependencyInjection;
using SSRD.AdminUI.Template;

namespace SSRD.IdentityUI.Admin
{
    public static class IdentityAdminExtension
    {
        /// <summary>
        /// Registers services for IdentityUI.Admin
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddIdentityAdmin(this IdentityUIServicesBuilder builder)
        {
            PagePath.LOGOUT = builder.IdentityManagementEndpoints.Logout;
            PagePath.MANAGE = builder.IdentityManagementEndpoints.Manage;
            PagePath.HOME = builder.IdentityManagementEndpoints.Home;

            builder.Services.AddAdminTemplate();

            builder.Services.ConfigureOptions(typeof(UIConfigureOptions));

            builder.Services.AddSingleton<IValidator<DataTableRequest>, DataTableValidator>();

            builder.Services.AddScoped<IUserDataService, UserDataService>();
            builder.Services.AddScoped<IRoleDataService, RoleDataService>();

            return builder;
        }

        /// <summary>
        /// Adds IdentityUI.Admin to the specified Microsoft.AspNetCore.Builder.IApplicationBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIAppBuilder UseIdentityAdmin(this IdentityUIAppBuilder builder)
        {
#if NET_CORE2
            builder.App.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "IdentityAdmin",
                    areaName: "IdentityAdmin",
                    template: "IdentityAdmin/{controller=Home}/{action=Index}/{id?}");
            });
#endif
#if NET_CORE3
            builder.App.UseEndpoints(endpoints => 
            {
                endpoints.MapAreaControllerRoute(
                name: "areas",
                areaName: "IdentityAdmin",
                pattern: "IdentityAdmin/{controller=Home}/{action=Index}/{id?}");
            });
#endif

            return builder;
        }
    }
}
