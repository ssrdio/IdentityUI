using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.DependencyInjection;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect
{
    internal static class OpenidConnectExtensions
    {
        public static IdentityUIServicesBuilder AddOpenidConnect(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddScoped<IClientDataService, ClientDataService>();
            builder.Services.AddScoped<IClientScopeDataService, ClientScopeDataService>();

            builder.Services.AddSingleton<IValidator<ClientConsentTableFilterModel>, ClientConsentTableFilterModelValidator>();
            builder.Services.AddSingleton<IValidator<ClientTokenTableFilterModel>, ClientTokenTableFilterModelValidator>();

            return builder;
        }

    }
}
