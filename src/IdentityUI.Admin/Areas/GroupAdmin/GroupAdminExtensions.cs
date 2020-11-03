using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services;
using SSRD.IdentityUI.Core.DependencyInjection;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin
{
    public static class GroupAdminExtensions
    {
        public static IdentityUIServicesBuilder AddGroupAdmin(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddScoped<IGroupAdminDashboardService, GroupAdminDashboardDataService>();
            builder.Services.AddScoped<IGroupAdminUserDataService, GroupAdminUserDataService>();
            builder.Services.AddScoped<IGroupAdminAuditDataService, GroupAdminAuditDataService>();
            builder.Services.AddScoped<IGroupAdminInviteDataService, GroupAdminInviteDataService>();
            builder.Services.AddScoped<IGroupAdminAttributeDataService, GroupAdminAttributeDataService>();
            builder.Services.AddScoped<IGroupAdminSettingsDataService, GroupAdminSettingsDataService>();

            builder.Services.AddTransient<IValidator<GroupAdminAuditTableRequest>, GroupAdminAuditTableRequestValidator>();

            return builder;
        }

#if NET_CORE2
        public static void MapGroupAdmin(this IRouteBuilder routes)
        {
            routes.MapAreaRoute(
                name: "GroupAdmin",
                areaName: "GroupAdmin",
                template: "GroupAdmin/{controller=Home}/{action=Index}/{id?}");
        }
#elif NET_CORE3
        public static void MapGroupAdmin(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapAreaControllerRoute(
                name: "areas",
                areaName: "GroupAdmin",
                pattern: "GroupAdmin/{controller=Home}/{action=Index}/{id?}");
        }
#endif
    }
}
