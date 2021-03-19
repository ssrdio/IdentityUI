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
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictExceptions;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public class ClientScopeService : IClientScopeService
    {
        private const string CLIENT_SCOPE_NOT_FOUND = "client_scope_not_found";
        private const string FAILED_TO_ADD_CLIENT_SCOPE = "failed_to_add_client_scope";
        private const string FAILED_TO_UPDATE_CLIENT_SCOPE = "failed_to_update_client_scope";
        private const string FAILED_TP_REMOVE_CLIENT_SCOPE = "failed_tp_remove_client_scope";

        private readonly IBaseDAO<ClientScopeEntity> _clientScopeDAO;
        private readonly OpenIddictScopeManager<ClientScopeEntity> _openIddictScopeManager;

        private readonly ILogger<ClientScopeService> _logger;

        public ClientScopeService(
            IBaseDAO<ClientScopeEntity> clientScopeDAO,
            OpenIddictScopeManager<ClientScopeEntity> openIddictScopeManager,
            ILogger<ClientScopeService> logger)
        {
            _clientScopeDAO = clientScopeDAO;
            _openIddictScopeManager = openIddictScopeManager;
            _logger = logger;
        }

        public async Task<Result> Add(AddClientScopeModel addClientScopeModel)
        {
            OpenIddictScopeDescriptor openIddictScopeDescriptor = new OpenIddictScopeDescriptor
            {
                Name = addClientScopeModel.Name
            };

            ClientScopeEntity clientScope = new ClientScopeEntity();

            await _openIddictScopeManager.PopulateAsync(clientScope, openIddictScopeDescriptor);

            try
            {
                await _openIddictScopeManager.CreateAsync(clientScope);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, $"Failed to add ClientScope, because of a validation exception");
                return Result.Fail(ex.Results.ToResultError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add ClientScope, because of a validation exception");
                return Result.Fail(FAILED_TO_ADD_CLIENT_SCOPE);
            }

            return Result.Ok();
        }

        public async Task<Result> Remove(string id)
        {
            IBaseSpecification<ClientScopeEntity, ClientScopeEntity> specification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .Where(x => x.Id == id)
                .Build();

            ClientScopeEntity clientScope = await _clientScopeDAO.SingleOrDefault(specification);
            if(clientScope == null)
            {
                _logger.LogError($"Client Scope not found. ClientScopeId {id}");
                return Result.Fail(CLIENT_SCOPE_NOT_FOUND);
            }

            bool result = await _clientScopeDAO.Remove(clientScope);
            if(!result)
            {
                _logger.LogError($"Failed to remove ClientScope. ClientScopeId {id}");
                return Result.Fail(FAILED_TP_REMOVE_CLIENT_SCOPE);
            }

            return Result.Ok();
        }

        public async Task<Result<ClientScopeEntity>> Update(string id, UpdateClientScopeModel updateClientScopeModel)
        {
            IBaseSpecification<ClientScopeEntity, ClientScopeEntity> specification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .Where(x => x.Id == id)
                .Build();

            ClientScopeEntity clientScope = await _clientScopeDAO.SingleOrDefault(specification, true);
            if (clientScope == null)
            {
                _logger.LogError($"Client Scope not found. ClientScopeId {id}");
                return Result.Fail<ClientScopeEntity>(CLIENT_SCOPE_NOT_FOUND);
            }

            clientScope.Name = updateClientScopeModel.Name;
            clientScope.DisplayName = updateClientScopeModel.DisplayName;
            clientScope.Description = updateClientScopeModel.Description;

            try
            {
                await _openIddictScopeManager.UpdateAsync(clientScope);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, $"Failed to update ClientScope, because of a validation exception");
                return Result.Fail<ClientScopeEntity>(ex.Results.ToResultError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update ClientScope, because of a validation exception");
                return Result.Fail<ClientScopeEntity>(FAILED_TO_ADD_CLIENT_SCOPE);
            }

            return Result.Ok(clientScope);
        }
    }
}
