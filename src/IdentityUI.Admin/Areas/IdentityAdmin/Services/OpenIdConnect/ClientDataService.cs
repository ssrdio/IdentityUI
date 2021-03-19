using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Services.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect
{
    public class ClientDataService : IClientDataService
    {
        private const string CLIENT_NOT_FOUND = "client_not_found";

        private readonly IBaseDAO<ClientEntity> _clientDAO;
        private readonly IBaseDAO<ClientScopeEntity> _clientScopeDAO;
        private readonly IBaseDAO<ClientConsentEntity> _clientConsentsDAO;
        private readonly IBaseDAO<ClientTokenEntity> _clientTokenDAO;

        private readonly IOpenIddictApplicationStore<ClientEntity> _openIddictApplicationStore;

        private readonly ILogger<ClientDataService> _logger;

        public ClientDataService(
            IBaseDAO<ClientEntity> clientDAO,
            IBaseDAO<ClientScopeEntity> clientScopeDAO,
            IBaseDAO<ClientConsentEntity> clientConsentsDAO,
            IBaseDAO<ClientTokenEntity> clientTokenDAO,
            IOpenIddictApplicationStoreResolver openIddictApplicationStoreResolver,
            ILogger<ClientDataService> logger)
        {
            _clientDAO = clientDAO;
            _clientScopeDAO = clientScopeDAO;
            _clientConsentsDAO = clientConsentsDAO;
            _clientTokenDAO = clientTokenDAO;
            _openIddictApplicationStore = openIddictApplicationStoreResolver.Get<ClientEntity>();
            _logger = logger;
        }

        public async Task<Result<ClientMenuViewModel>> GetMenuViewModel(string id)
        {
            IBaseSpecification<ClientEntity, ClientMenuViewModel> specification = SpecificationBuilder
                .Create<ClientEntity>()
                .Where(x => x.Id == id)
                .Select(x => new ClientMenuViewModel(
                    x.Id,
                    x.DisplayName))
                .Build();

            ClientMenuViewModel client = await _clientDAO.SingleOrDefault(specification);
            if(client == null)
            {
                _logger.LogError($"Client not found. ClientId {id}");
                return Result.Fail<ClientMenuViewModel>(CLIENT_NOT_FOUND);
            }

            return Result.Ok(client);
        }

        public async Task<Result<ClientDetailsViewModel>> GetDetailsViewModel(string id)
        {
            Result<ClientMenuViewModel> clientMenuResult = await GetMenuViewModel(id);
            if(clientMenuResult.Failure)
            {
                return Result.Fail<ClientDetailsViewModel>(clientMenuResult);
            }

            ClientDetailsViewModel clientDetailsViewModel = new ClientDetailsViewModel(
                clientMenu: clientMenuResult.Value,
                endpoints: OpenIdConnectConstants.ENDPOINTS,
                grantTypes: OpenIdConnectConstants.GRANT_TYPES,
                responseTypes: OpenIdConnectConstants.RESPONSE_TYPES);

            return Result.Ok(clientDetailsViewModel);
        }

        public async Task<Result<ClientConsentViewModel>> GetConsentViewModel(string id)
        {
            Result<ClientMenuViewModel> clientMenuResult = await GetMenuViewModel(id);
            if(clientMenuResult.Failure)
            {
                return Result.Fail<ClientConsentViewModel>(clientMenuResult);
            }

            List<Select2Item> statuses = new List<Select2Item>
            {
                new Select2Item(OpenIddictConstants.Statuses.Inactive),
                new Select2Item(OpenIddictConstants.Statuses.Redeemed),
                new Select2Item(OpenIddictConstants.Statuses.Rejected),
                new Select2Item(OpenIddictConstants.Statuses.Revoked),
                new Select2Item(OpenIddictConstants.Statuses.Valid)
            };

            ClientConsentViewModel clientConsentViewModel = new ClientConsentViewModel(
                statuses: statuses,
                clientMenu: clientMenuResult.Value);

            return Result.Ok(clientConsentViewModel);
        }

        public async Task<Result<ClientTokenViewModel>> GetTokenViewModel(string id)
        {
            Result<ClientMenuViewModel> clientMenuResult = await GetMenuViewModel(id);
            if (clientMenuResult.Failure)
            {
                return Result.Fail<ClientTokenViewModel>(clientMenuResult);
            }

            List<Select2Item> statuses = new List<Select2Item>
            {
                new Select2Item(OpenIddictConstants.Statuses.Inactive),
                new Select2Item(OpenIddictConstants.Statuses.Redeemed),
                new Select2Item(OpenIddictConstants.Statuses.Rejected),
                new Select2Item(OpenIddictConstants.Statuses.Revoked),
                new Select2Item(OpenIddictConstants.Statuses.Valid)
            };

            List<Select2Item> types = new List<Select2Item>
            {
                new Select2Item(OpenIddictConstants.TokenTypeHints.AccessToken),
                new Select2Item(OpenIddictConstants.TokenTypeHints.AuthorizationCode),
                new Select2Item(OpenIddictConstants.TokenTypeHints.DeviceCode),
                new Select2Item(OpenIddictConstants.TokenTypeHints.IdToken),
                new Select2Item(OpenIddictConstants.TokenTypeHints.RefreshToken),
                new Select2Item(OpenIddictConstants.TokenTypeHints.UserCode),
            };

            ClientTokenViewModel clientConsentViewModel = new ClientTokenViewModel(
                id: clientMenuResult.Value.Id,
                name: clientMenuResult.Value.Name,
                statuses: statuses,
                types: types);

            return Result.Ok(clientConsentViewModel);
        }

        public async Task<Result<DataTableResult<ClientTableModel>>> Get(DataTableRequest dataTableRequest)
        {
            ISelectSpecificationBuilder<ClientEntity, ClientTableModel> specificationBuilder = SpecificationBuilder
                .Create<ClientEntity>()
                .SearchByName(dataTableRequest.Search)
                .Select(x => new ClientTableModel(
                    x.Id,
                    x.DisplayName,
                    x.Type));

            DataTableResult<ClientTableModel> dataTableResult = await _clientDAO.Get(specificationBuilder, dataTableRequest);

            return Result.Ok(dataTableResult);
        }

        public async Task<Result<ClientDetailsModel>> Get(string id)
        {
            IBaseSpecification<ClientEntity, ClientEntity> specification = SpecificationBuilder
                .Create<ClientEntity>()
                .Where(x => x.Id == id)
                .Build();

            ClientEntity client = await _clientDAO.SingleOrDefault(specification);
            if(client == null)
            {
                _logger.LogInformation($"OpenIddictApplication not found. Id {id}");
                return Result.Fail<ClientDetailsModel>(CLIENT_NOT_FOUND);
            }

            return await Get(client);
        }

        public async Task<Result<ClientDetailsModel>> Get(ClientEntity client)
        {
            ImmutableArray<string> redirectUris = await _openIddictApplicationStore.GetRedirectUrisAsync(client, new CancellationToken());
            ImmutableArray<string> postLogoutRedirectUris = await _openIddictApplicationStore.GetPostLogoutRedirectUrisAsync(client, new CancellationToken());
            ImmutableArray<string> permissions = await _openIddictApplicationStore.GetPermissionsAsync(client, new CancellationToken());

            List<string> endpoints = new List<string>();
            List<string> grantTypes = new List<string>();
            List<string> responseTypes = new List<string>();

            foreach (string permission in permissions)
            {
                if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Endpoint))
                {
                    endpoints.Add(permission.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""));
                }
                else if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType))
                {
                    grantTypes.Add(permission.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""));
                }
                else if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.ResponseType))
                {
                    responseTypes.Add(permission.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""));
                }
            }

            ImmutableArray<string> requirements = await _openIddictApplicationStore.GetRequirementsAsync(client, new CancellationToken());
            bool requirePkce = requirements.Contains(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);

            string consentType = await _openIddictApplicationStore.GetConsentTypeAsync(client, new CancellationToken());
            bool requireConsent = consentType == OpenIddictConstants.ConsentTypes.Explicit;

            ClientDetailsModel clientDetailsModel = new ClientDetailsModel(
                id: client.Id,
                name: client.DisplayName,
                redirectUrls: redirectUris,
                postLogoutUrls: postLogoutRedirectUris,
                endpoints: endpoints,
                grantTypes: grantTypes,
                responseTypes: responseTypes,
                requireConsent: requireConsent,
                requirePkce: requirePkce);

            return Result.Ok(clientDetailsModel);
        }

        public async Task<Result<ClientIdModel>> GetClientId(string id)
        {
            IBaseSpecification<ClientEntity, ClientIdModel> specification = SpecificationBuilder
                .Create<ClientEntity>()
                .Where(x => x.Id == id)
                .Select(x => new ClientIdModel(
                    x.ClientId))
                .Build();

            ClientIdModel client = await _clientDAO.SingleOrDefault(specification);
            if(client == null)
            {
                _logger.LogError($"Client not found. ClientId {id}");
                return Result.Fail<ClientIdModel>(CLIENT_NOT_FOUND);
            }

            return Result.Ok(client);
        }

        public async Task<Result<ClientSecretModel>> GetClientSecret(string id)
        {
            IBaseSpecification<ClientEntity, ClientSecretModel> specification = SpecificationBuilder
                .Create<ClientEntity>()
                .Where(x => x.Id == id)
                .Select(x => new ClientSecretModel(
                    x.ClientSecret != null))
                .Build();

            ClientSecretModel client = await _clientDAO.SingleOrDefault(specification);
            if (client == null)
            {
                _logger.LogError($"Client not found. ClientId {id}");
                return Result.Fail<ClientSecretModel>(CLIENT_NOT_FOUND);
            }

            return Result.Ok(client);
        }

        public async Task<Result<ClientScopesModel>> GetScopes(string id)
        {
            var getClientPermissionsSpecification = SpecificationBuilder
                .Create<ClientEntity>()
                .Where(x => x.Id == id)
                .Select(x => new ClientEntity
                    {
                        Id = x.Id,
                        Permissions = x.Permissions
                    })
                .Build();

            ClientEntity clientEntity = await _clientDAO.SingleOrDefault(getClientPermissionsSpecification);
            if(clientEntity == null)
            {
                _logger.LogError($"Client not found. ClientId {id}");
                return Result.Fail<ClientScopesModel>(CLIENT_NOT_FOUND);
            }

            ImmutableArray<string> permissionList = await _openIddictApplicationStore.GetPermissionsAsync(clientEntity, new CancellationToken());

            List<string> assignedScopes = new List<string>();
            foreach(var permission in permissionList)
            {
                if (permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope))
                {
                    assignedScopes.Add(permission.Replace(OpenIddictConstants.Permissions.Prefixes.Scope, ""));
                }
            }

            var getAvaibleScopesSpecification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .Where(x => !assignedScopes.Contains(x.Name))
                .Select(x => x.Name)
                .Build();

            List<string> avaibleScopes = await _clientScopeDAO.Get(getAvaibleScopesSpecification);

            ClientScopesModel clientScopesModel = new ClientScopesModel(
                assigned: assignedScopes,
                available: avaibleScopes);

            return Result.Ok(clientScopesModel);
        }

        public async Task<Result<Select2Result<Select2Item>>> GetConsentSubjects(string id, Select2Request select2Request)
        {
            ISelectSpecificationBuilder<ClientConsentEntity, string> specification = SpecificationBuilder
                .Create<ClientConsentEntity>()
                .Where(x => x.Application.Id == id)
                .SearchBySubject(select2Request.Term)
                .Select(x => x.Subject)
                .Distinct()
                .OrderByAssending(x => x);

            Select2Result<Select2Item> select2Result = await _clientConsentsDAO.Get(specification, select2Request);

            return Result.Ok(select2Result);
        }

        public async Task<Result<Select2Result<Select2Item>>> GetTokenSubjects(string id, Select2Request select2Request)
        {
            ISelectSpecificationBuilder<ClientTokenEntity, string> specification = SpecificationBuilder
                .Create<ClientTokenEntity>()
                .Where(x => x.Application.Id == id)
                .SearchBySubject(select2Request.Term)
                .Select(x => x.Subject)
                .Distinct()
                .OrderByAssending(x => x);

            Select2Result<Select2Item> select2Result = await _clientTokenDAO.Get(specification, select2Request);

            return Result.Ok(select2Result);
        }

        public async Task<Result<DataTableResult<ClientConsentTableModel>>> GetConsents(
            string id,
            DataTableRequest dataTableRequest,
            ClientConsentTableFilterModel clientConsentTableFilterModel)
        {
            var specification = SpecificationBuilder
                .Create<ClientConsentEntity>()
                .Where(x => x.Application.Id == id)
                .WithStatus(clientConsentTableFilterModel.Status)
                .WithSubject(clientConsentTableFilterModel.Subject)
                .WithScope(clientConsentTableFilterModel.Scope)
                .InRange(clientConsentTableFilterModel.From?.UtcDateTime, clientConsentTableFilterModel.To?.UtcDateTime)
                .Select(x => new ClientConsentTableModel(
                    x.Id,
                    x.Scopes,
                    x.Status,
                    x.Subject,
                    x.Type,
                    x.CreationDate));

            DataTableResult<ClientConsentTableModel> result = await _clientConsentsDAO.Get(specification, dataTableRequest);

            return Result.Ok(result);
        }

        public async Task<Result<DataTableResult<ClientTokenTableModel>>> GetTokens(
            string id,
            DataTableRequest dataTableRequest,
            ClientTokenTableFilterModel clientTokenTableFilter)
        {
            var specification = SpecificationBuilder
                .Create<ClientTokenEntity>()
                .Where(x => x.Application.Id == id)
                .WithStatus(clientTokenTableFilter.Status)
                .WithType(clientTokenTableFilter.Type)
                .WithSubject(clientTokenTableFilter.Subject)
                .InCreationDateRange(clientTokenTableFilter.From?.UtcDateTime, clientTokenTableFilter.To?.UtcDateTime)
                .Select(x => new ClientTokenTableModel(
                    x.Id,
                    x.Status,
                    x.Type,
                    x.Subject,
                    x.CreationDate,
                    x.ExpirationDate,
                    x.RedemptionDate));

            DataTableResult<ClientTokenTableModel> result = await _clientTokenDAO.Get(specification, dataTableRequest);

            return Result.Ok(result);
        }
    }

    public static class OpenIddictEntityFrameworkCoreApplicationExtensions
    {
        public static IBaseSpecificationBuilder<ClientEntity> SearchByName(
            this IBaseSpecificationBuilder<ClientEntity> builder,
            string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return builder;
            }

            builder = builder
                .Where(x => x.DisplayName.ToUpper() == search.ToUpper());

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientScopeEntity> SearchByName(
            this IBaseSpecificationBuilder<ClientScopeEntity> builder,
            string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return builder;
            }

            builder = builder
                .Where(x => x.Name.ToUpper() == search.ToUpper());

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientConsentEntity> WithStatus(
            this IBaseSpecificationBuilder<ClientConsentEntity> builder,
            string status)
        {
            if(string.IsNullOrEmpty(status))
            {
                return builder;
            }

            builder = builder.Where(x => x.Status == status);

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientConsentEntity> WithSubject(
            this IBaseSpecificationBuilder<ClientConsentEntity> builder,
            string subject)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return builder;
            }

            builder = builder.Where(x => x.Subject == subject);

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientConsentEntity> WithScope(
            this IBaseSpecificationBuilder<ClientConsentEntity> builder,
            string scope)
        {
            if (string.IsNullOrEmpty(scope))
            {
                return builder;
            }

            builder = builder.Where(x => x.Scopes.Contains($"\"{scope}\""));

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientConsentEntity> SearchBySubject(
            this IBaseSpecificationBuilder<ClientConsentEntity> builder,
            string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => x.Subject.ToUpper().Contains(search));

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientConsentEntity> InRange(
            this IBaseSpecificationBuilder<ClientConsentEntity> builder,
            DateTime? from,
            DateTime? to)
        {
            if(from.HasValue)
            {
                if (from.Value.Kind != DateTimeKind.Utc)
                {
                    throw new Exception($"From time is not in utc");
                }

                builder = builder.Where(x => x.CreationDate >= from);
            }

            if (to.HasValue)
            {
                if (to.Value.Kind != DateTimeKind.Utc)
                {
                    throw new Exception($"To time is not in utc");
                }

                builder = builder.Where(x => x.CreationDate < to);
            }

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientTokenEntity> SearchBySubject(
            this IBaseSpecificationBuilder<ClientTokenEntity> builder,
            string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => x.Subject.ToUpper().Contains(search));

            return builder;
        }

        public static IBaseSpecificationBuilder<ClientTokenEntity> WithStatus(
            this IBaseSpecificationBuilder<ClientTokenEntity> builder,
            string status)
        {
            if(string.IsNullOrEmpty(status))
            {
                return builder;
            }

            //status = status.ToUpper();

            return builder
                .Where(x => x.Status == status);
        }

        public static IBaseSpecificationBuilder<ClientTokenEntity> WithType(
            this IBaseSpecificationBuilder<ClientTokenEntity> builder,
            string type)
        {
            if(string.IsNullOrEmpty(type))
            {
                return builder;
            }

            return builder
                .Where(x => x.Type == type);
        }

        public static IBaseSpecificationBuilder<ClientTokenEntity> WithSubject(
            this IBaseSpecificationBuilder<ClientTokenEntity> builder,
            string subject)
        {
            if(string.IsNullOrEmpty(subject))
            {
                return builder;
            }

            return builder
                .Where(x => x.Subject == subject);
        }

        public static IBaseSpecificationBuilder<ClientTokenEntity> InCreationDateRange(
            this IBaseSpecificationBuilder<ClientTokenEntity> builder,
            DateTime? from,
            DateTime? to)
        {
            if (from.HasValue)
            {
                if (from.Value.Kind != DateTimeKind.Utc)
                {
                    throw new Exception($"From time is not in utc");
                }

                builder = builder.Where(x => x.CreationDate >= from);
            }

            if (to.HasValue)
            {
                if (to.Value.Kind != DateTimeKind.Utc)
                {
                    throw new Exception($"To time is not in utc");
                }

                builder = builder.Where(x => x.CreationDate < to);
            }

            return builder;
        }
    }
}
