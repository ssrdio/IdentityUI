using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictExceptions;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class AddClientService : IAddClientService
    {
        private const string FAILED_TO_ADD_CLIENT = "failed_to_add_client";

        private readonly OpenIddictApplicationManager<ClientEntity> _openIddictApplicationManager;

        private readonly ILogger<AddClientService> _logger;

        public AddClientService(
            OpenIddictApplicationManager<ClientEntity> openIddictApplicationManager,
            ILogger<AddClientService> logger)
        {
            _openIddictApplicationManager = openIddictApplicationManager;
            _logger = logger;
        }

        public Task<Result<IdStringModel>> AddSinglePageClient(AddSinglePageClientModel addSinglePageClientModel)
        {
            OpenIddictApplicationDescriptor applicationDescriptor = new OpenIddictApplicationDescriptor
            {
                DisplayName = addSinglePageClientModel.Name,
                ClientId = addSinglePageClientModel.ClientId,

                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Token
                },
                Requirements =
                {
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                }
            };

            foreach(string redirectUri in addSinglePageClientModel.RedirectUrls)
            {
                applicationDescriptor.RedirectUris.Add(new Uri(redirectUri));
            }

            foreach(string postLogoutUri in addSinglePageClientModel.PostLogoutUrls)
            {
                applicationDescriptor.PostLogoutRedirectUris.Add(new Uri(postLogoutUri));
            }

            return AddClient(applicationDescriptor, null);
        }

        public Task<Result<IdStringModel>> AddWebAppClient(AddWebAppClientModel addWebAppClientModel)
        {
            OpenIddictApplicationDescriptor applicationDescriptor = new OpenIddictApplicationDescriptor
            {
                DisplayName = addWebAppClientModel.Name,
                ClientId = addWebAppClientModel.ClientId,

                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Logout,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,

                    OpenIddictConstants.Permissions.ResponseTypes.Code
                }
            };

            foreach (string redirectUrl in addWebAppClientModel.RedirectUrls)
            {
                applicationDescriptor.RedirectUris.Add(new Uri(redirectUrl));
            }

            foreach (string postLogoutUrl in addWebAppClientModel.PostLogoutUrls)
            {
                applicationDescriptor.PostLogoutRedirectUris.Add(new Uri(postLogoutUrl));
            }

            return AddClient(applicationDescriptor, null);
        }

        public Task<Result<IdStringModel>> AddClientCredentials(AddClientCredentialsClientModel addClientCredentialsClientModel)
        {
            OpenIddictApplicationDescriptor applicationDescriptor = new OpenIddictApplicationDescriptor
            {
                DisplayName = addClientCredentialsClientModel.Name,
                ClientId = addClientCredentialsClientModel.ClientId,

                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                },
            };

            return AddClient(applicationDescriptor, addClientCredentialsClientModel.ClientSecret);
        }

        public Task<Result<IdStringModel>> AddMobileClient(AddMobileClientModel addMobileClient)
        {
            OpenIddictApplicationDescriptor applicationDescriptor = new OpenIddictApplicationDescriptor
            {
                DisplayName = addMobileClient.Name,
                ClientId = addMobileClient.ClientId,

                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Token
                },
                Requirements =
                {
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                },
                
            };

            foreach (string redirectUri in addMobileClient.RedirectUrls)
            {
                applicationDescriptor.RedirectUris.Add(new Uri(redirectUri));
            }

            foreach (string postLogoutUri in addMobileClient.PostLogoutUrls)
            {
                applicationDescriptor.PostLogoutRedirectUris.Add(new Uri(postLogoutUri));
            }

            return AddClient(applicationDescriptor, null);
        }

        public Task<Result<IdStringModel>> AddCustomClient(AddCustomClientModel addCustomClient)
        {
            OpenIddictApplicationDescriptor applicationDescriptor = new OpenIddictApplicationDescriptor
            {
                DisplayName = addCustomClient.Name,
                ClientId = addCustomClient.ClientId
            };

            return AddClient(applicationDescriptor, null);
        }

        private async Task<Result<IdStringModel>> AddClient(OpenIddictApplicationDescriptor applicationDescriptor, string secret)
        {
            ClientEntity client = new ClientEntity();

            await _openIddictApplicationManager.PopulateAsync(client, applicationDescriptor);

            try
            {
                await _openIddictApplicationManager.CreateAsync(client, secret);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, $"Failed to add ClientCredentials client, because of a validation exception");
                return Result.Fail<IdStringModel>(ex.Results.ToResultError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add ClientCredentials client, because of a validation exception");
                return Result.Fail<IdStringModel>(FAILED_TO_ADD_CLIENT);
            }

            IdStringModel idStringModel = new IdStringModel(
                id: client.Id);

            return Result.Ok(idStringModel);
        }
    }
}
