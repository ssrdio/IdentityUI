using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictExceptions;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class ManageClientService : IManageClientService
    {
        private const string CLIENT_NOT_FOUND = "client_not_found";
        private const string FAILED_TO_REMOVE_CLIENT = "failed_to_remove_client";

        private const string FAILED_TO_UPDATE_CLIENT = "failed_to_update_client";

        private const string CLIENT_ALREADY_HAS_SECRET = "client_already_has_secret";
        private const string CLIENT_DOES_NOT_HAVE_A_SECRET = "client_does_not_have_a_secret";

        private readonly IBaseDAO<ClientEntity> _clientDAO;
        private readonly IBaseDAO<ClientScopeEntity> _clientScopeDAO;

        private readonly IOpenIddictApplicationStore<ClientEntity> _openIddictClientStore;
        private readonly OpenIddictApplicationManager<ClientEntity> _openIddictClientManager;

        private readonly ILogger<ManageClientService> _logger;

        public ManageClientService(
            IBaseDAO<ClientEntity> clientDAO,
            IBaseDAO<ClientScopeEntity> clientScopeDAO,
            IOpenIddictApplicationStoreResolver openIddictClientStoreResolver,
            OpenIddictApplicationManager<ClientEntity> openIddictClientManager,
            ILogger<ManageClientService> logger)
        {
            _clientDAO = clientDAO;
            _clientScopeDAO = clientScopeDAO;

            _openIddictClientStore = openIddictClientStoreResolver.Get<ClientEntity>();
            _openIddictClientManager = openIddictClientManager;
            _logger = logger;
        }

        private async Task<Result<ClientEntity>> Get(string id)
        {
            IBaseSpecification<ClientEntity, ClientEntity> specification = SpecificationBuilder
                .Create<ClientEntity>()
                .Where(x => x.Id == id)
                .Build();

            ClientEntity client = await _clientDAO.SingleOrDefault(specification, withTracking: true);
            if (client == null)
            {
                _logger.LogError($"Client not found. ClientId {id}");
                return Result.Fail<ClientEntity>(CLIENT_NOT_FOUND);
            }

            return Result.Ok(client);
        }

        public async Task<Result> Remove(string id)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if(getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            client.ClientId = $"{client}_deleted_{Guid.NewGuid()}";

            bool updateResult = await _clientDAO.Update(client);
            if(updateResult)
            {
                _logger.LogError($"Failed to update Client for deletion. Client id {id}");
                return Result.Fail(FAILED_TO_REMOVE_CLIENT);
            }

            //TODO: try to do that in transaction
            bool removeResult = await _clientDAO.Remove(client);
            if(!removeResult)
            {
                _logger.LogError($"Failed to remove Client. ClientId {id}");
                return Result.Fail(FAILED_TO_REMOVE_CLIENT);
            }

            return Result.Ok();
        }

        private async Task<Result<ClientEntity>> Update(ClientEntity client)
        {
            try
            {
                await _openIddictClientManager.UpdateAsync(client);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, $"Failed to add ClientCredentials client, because of a validation exception");
                return Result.Fail<ClientEntity>(ex.Results.ToResultError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add ClientCredentials client, because of a validation exception");
                return Result.Fail<ClientEntity>(FAILED_TO_UPDATE_CLIENT);
            }

            return Result.Ok(client);
        }

        private async Task<Result<ClientEntity>> Update(ClientEntity client, string secret)
        {
            try
            {
                await _openIddictClientManager.UpdateAsync(client, secret);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, $"Failed to add ClientCredentials client, because of a validation exception");
                return Result.Fail<ClientEntity>(ex.Results.ToResultError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add ClientCredentials client, because of a validation exception");
                return Result.Fail<ClientEntity>(FAILED_TO_UPDATE_CLIENT);
            }

            return Result.Ok(client);
        }

        public async Task<Result<ClientEntity>> Update(string id, UpdateClientModel updateClientModel)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail<ClientEntity>(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            client.DisplayName = updateClientModel.Name;

            await _openIddictClientStore.SetRedirectUrisAsync(
                client,
                ImmutableArray.Create(updateClientModel.RedirectUrls.ToArray()),
                new CancellationToken());

            await _openIddictClientStore.SetPostLogoutRedirectUrisAsync(
                client,
                ImmutableArray.Create(updateClientModel.PostLogoutUrls.ToArray()),
                new CancellationToken());

            ImmutableArray<string> immutablePermissions = await _openIddictClientStore.GetPermissionsAsync(client, new CancellationToken());

            List<string> permissions = immutablePermissions
                .Where(x => !x.StartsWith(OpenIddictConstants.Permissions.Prefixes.Endpoint))
                .Where(x => !x.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType))
                .Where(x => !x.StartsWith(OpenIddictConstants.Permissions.Prefixes.ResponseType))
                .ToList();

            foreach(string endpint in updateClientModel.Endpoints)
            {
                bool isValid = OpenIdConnectConstants.ENDPOINTS.Contains(endpint);
                if(!isValid)
                {
                    _logger.LogWarning($"Client endpoint is not valid. ClientId {id}, endpoint {endpint}");
                    continue;
                }

                permissions.Add(OpenIddictConstants.Permissions.Prefixes.Endpoint + endpint);
            }

            foreach(string grantType in updateClientModel.GrantTypes)
            {
                bool isValid = OpenIdConnectConstants.GRANT_TYPES.Contains(grantType);
                if(!isValid)
                {
                    _logger.LogWarning($"Client grant type is not valid. CleintId {id}, grantType {grantType}");
                    continue;
                }

                permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + grantType);
            }

            foreach (string responseType in updateClientModel.ResponseTypes)
            {
                bool isValid = OpenIdConnectConstants.RESPONSE_TYPES.Contains(responseType);
                if (!isValid)
                {
                    _logger.LogWarning($"Client response type is not valid. CleintId {id}, responseType {responseType}");
                    continue;
                }

                permissions.Add(OpenIddictConstants.Permissions.Prefixes.ResponseType + responseType);
            }


            await _openIddictClientStore.SetPermissionsAsync(
                client,
                ImmutableArray.Create(permissions.ToArray()),
                new CancellationToken());

            ImmutableArray<string> immutableRequirements = await _openIddictClientStore.GetRequirementsAsync(client, new CancellationToken());

            List<string> requirements = immutableRequirements
                .Where(x => !x.StartsWith(OpenIddictConstants.Requirements.Prefixes.Feature))
                .ToList();

            if(updateClientModel.RequirePkce)
            {
                requirements.Add(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);
            }

            await _openIddictClientStore.SetRequirementsAsync(
                client,
                ImmutableArray.Create(requirements.ToArray()),
                new CancellationToken());

            if(updateClientModel.RequireConsent)
            {
                await _openIddictClientStore.SetConsentTypeAsync(client, OpenIddictConstants.ConsentTypes.Explicit, new CancellationToken());
            }
            else
            {
                await _openIddictClientStore.SetConsentTypeAsync(client, OpenIddictConstants.ConsentTypes.Implicit, new CancellationToken());
            }

            return await Update(client);
        }

        private async Task<List<string>> GetScopes(List<string> requiredScopes)
        {
            IBaseSpecification<ClientScopeEntity, string> specification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .Where(x => requiredScopes.Contains(x.Name))
                .Select(x => x.Name)
                .Build();

            List<string> validScopes = await _clientScopeDAO.Get(specification);

            validScopes.AddRange(OpenIdConnectConstants.SCOPES);

            return validScopes;
        }

        public async Task<Result> AddScopes(string id, ManageClientScopesModel clientScopes)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            ImmutableArray<string> immutablePermissions = await _openIddictClientStore.GetPermissionsAsync(client, new CancellationToken());

            List<string> validScopes = await GetScopes(clientScopes.Scopes);
            List<string> scopes = new List<string>(clientScopes.Scopes.Count);

            foreach(var scope in clientScopes.Scopes)
            {
                bool isValid = validScopes
                    .Where(x => x == scope)
                    .Any();

                if(!isValid)
                {
                    _logger.LogWarning($"Scope is not valid. ClientId {id} Scope {scope}");
                    continue;
                }

                string scopeWtihPerfix = OpenIddictConstants.Permissions.Prefixes.Scope + scope;

                if(immutablePermissions.Contains(scopeWtihPerfix))
                {
                    _logger.LogWarning($"Client already contains scope. ClientId {id}, Scope {scope}");
                    continue;
                }

                scopes.Add(scopeWtihPerfix);
            }

            List<string> permissions = new List<string>(immutablePermissions);
            permissions.AddRange(scopes);

            await _openIddictClientStore.SetPermissionsAsync(
                client,
                ImmutableArray.Create(permissions.ToArray()),
                new CancellationToken());

            return await Update(client);
        }

        public async Task<Result> RemoveScopes(string id, ManageClientScopesModel clientScopes)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            ImmutableArray<string> immutablePermissions = await _openIddictClientStore.GetPermissionsAsync(client, new CancellationToken());

            List<string> scopesToRemove = clientScopes.Scopes
                .Select(x => $"{OpenIddictConstants.Permissions.Prefixes.Scope}{x}")
                .ToList();

            List<string> permissions = new List<string>(immutablePermissions);

            foreach(string scopeToRemove in scopesToRemove)
            {
                int index = permissions.FindIndex(x => x == scopeToRemove);
                if(index == -1)
                {
                    _logger.LogWarning($"Scope to remove is not assigned. CleintId {id}, Scope {scopeToRemove}");
                    continue;
                }

                permissions.RemoveAt(index);
            }

            await _openIddictClientStore.SetPermissionsAsync(
                client,
                ImmutableArray.Create(permissions.ToArray()),
                new CancellationToken());

            return await Update(client);
        }

        public async Task<Result> UpdateClientId(string id, UpdateClientIdModel updateClientIdModel)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            client.ClientId = updateClientIdModel.ClientId;

            return await Update(client);
        }

        public async Task<Result> AddClientSecret(string id, AddClientSecretModel addClientSecretModel)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            if(client.ClientSecret != null)
            {
                _logger.LogError($"Client already has set secret. ClientId {id}");
                return Result.Fail(CLIENT_ALREADY_HAS_SECRET);
            }

            await _openIddictClientStore.SetClientTypeAsync(client, OpenIddictConstants.ClientTypes.Confidential, default);

            return await Update(client, addClientSecretModel.ClientSecret);
        }

        public async Task<Result> UpdateClientSecret(string id, UpdateClientSecretModel updateClientSecret)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            if(client.ClientSecret == null)
            {
                _logger.LogError($"Client does not have a secret. ClientId {id}");
                return Result.Fail(CLIENT_DOES_NOT_HAVE_A_SECRET);
            }

            return await Update(client, updateClientSecret.ClientSecret);
        }

        public async Task<Result> RemoveClientSecret(string id)
        {
            Result<ClientEntity> getClientResult = await Get(id);
            if (getClientResult.Failure)
            {
                return Result.Fail(getClientResult);
            }

            ClientEntity client = getClientResult.Value;

            if (client.ClientSecret == null)
            {
                _logger.LogError($"Client does not have a secret. ClientId {id}");
                return Result.Fail(CLIENT_DOES_NOT_HAVE_A_SECRET);
            }

            await _openIddictClientStore.SetClientSecretAsync(client, null, default);
            await _openIddictClientStore.SetClientTypeAsync(client, OpenIddictConstants.ClientTypes.Public, default);

            return await Update(client);
        }
    }
}
