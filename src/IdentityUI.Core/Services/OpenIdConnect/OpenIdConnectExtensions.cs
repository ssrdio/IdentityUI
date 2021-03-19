using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.DependencyInjection;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    internal static class OpenIdConnectExtensions
    {
        public static IdentityUIServicesBuilder AddOpenIdConnect(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddScoped<IAddClientService, AddClientService>();
            builder.Services.AddScoped<IManageClientService, ManageClientService>();

            builder.Services.AddScoped<IClientScopeService, ClientScopeService>();

            builder.Services.AddScoped<IClientConsentService, ClientConsentService>();
            builder.Services.AddScoped<IClientTokenService, ClientTokenService>();

            builder.Services.AddTransient<IBaseDAO<ClientEntity>, IdentityUIBaseDAO<ClientEntity>>();
            builder.Services.AddTransient<IBaseDAO<ClientScopeEntity>, IdentityUIBaseDAO<ClientScopeEntity>>();
            builder.Services.AddTransient<IBaseDAO<ClientConsentEntity>, IdentityUIBaseDAO<ClientConsentEntity>>();
            builder.Services.AddTransient<IBaseDAO<ClientTokenEntity>, IdentityUIBaseDAO<ClientTokenEntity>>();

            builder.AddValidators();

            return builder;
        }

        private static void AddValidators(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddSingleton<IValidator<Models.AddSinglePageClientModel>, Models.AddSinglePageClientModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.AddWebAppClientModel>, Models.AddWebAppClientModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.AddClientCredentialsClientModel>, Models.AddClientCredentialsClientModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.AddMobileClientModel>, Models.AddMobileClientModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.AddCustomClientModel>, Models.AddCustomClientModelValidator>();

            builder.Services.AddSingleton<IValidator<Models.UpdateClientModel>, Models.UpdateClientModelValidator>();

            builder.Services.AddSingleton<IValidator<Models.UpdateClientScopeModel>, Models.UpdateClientScopeModelValidator>();

            builder.Services.AddSingleton<IValidator<Models.ManageClientScopesModel>, Models.ManageClientScopesModelValidator>();

            builder.Services.AddSingleton<IValidator<Models.UpdateClientIdModel>, Models.UpdateClientIdModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.UpdateClientSecretModel>, Models.UpdateClientSecretModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.AddClientSecretModel>, Models.AddClientSecretModelValidator>();

            builder.Services.AddSingleton<IValidator<Models.AddClientScopeModel>, Models.AddClientScopeModelValidator>();
            builder.Services.AddSingleton<IValidator<Models.UpdateClientScopeModel>, Models.UpdateClientScopeModelValidator>();
        }
    }
}
