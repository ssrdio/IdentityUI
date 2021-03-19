using Microsoft.Extensions.Logging;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect
{
    public class ClientScopeDataService : IClientScopeDataService
    {
        private const string CLIENT_SCOPE_NOT_FOUND = "client_scope_not_found";

        private readonly IBaseDAO<ClientScopeEntity> _clientScopeDAO;

        private readonly ILogger<ClientScopeDataService> _logger;

        public ClientScopeDataService(IBaseDAO<ClientScopeEntity> clientScopeDAO, ILogger<ClientScopeDataService> logger)
        {
            _clientScopeDAO = clientScopeDAO;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<ClientScopeTableModel>>> Get(DataTableRequest dataTableRequest)
        {
            ISelectSpecificationBuilder<ClientScopeEntity, ClientScopeTableModel> specification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .SearchByName(dataTableRequest.Search)
                .Select(x => new ClientScopeTableModel(
                    x.Id,
                    x.Name));

            DataTableResult<ClientScopeTableModel> result = await _clientScopeDAO.Get(specification, dataTableRequest);

            return Result.Ok(result);
        }

        public async Task<Result<Select2Result<Select2Item>>> Get(Select2Request select2Request)
        {
            ISelectSpecificationBuilder<ClientScopeEntity, string> specification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .SearchByName(select2Request.Term)
                .Select(x => x.Name)
                .Distinct()
                .OrderByAssending(x => x);

            Select2Result<Select2Item> select2Result = await _clientScopeDAO.Get(specification, select2Request);

            if(select2Request.Page == 1)
            {
                List<Select2Item> systemScopes = OpenIdConnectConstants.SCOPES
                    .Select(x => new Select2Item(
                        x))
                    .ToList();

                systemScopes.AddRange(select2Result.Results);

                select2Result.Results = systemScopes;
            }

            return Result.Ok(select2Result);
        }

        public async Task<Result<ClientScopeDetailsModel>> Get(string id)
        {
            IBaseSpecification<ClientScopeEntity, ClientScopeDetailsModel> specification = SpecificationBuilder
                .Create<ClientScopeEntity>()
                .Where(x => x.Id == id)
                .Select(x => new ClientScopeDetailsModel(
                    x.Name,
                    x.DisplayName,
                    x.Description))
                .Build();

            ClientScopeDetailsModel clientScope = await _clientScopeDAO.SingleOrDefault(specification);
            if(clientScope == null)
            {
                _logger.LogError($"Client scope not found. ClientScope id {id}");
                return Result.Fail<ClientScopeDetailsModel>(CLIENT_SCOPE_NOT_FOUND);
            }

            return Result.Ok(clientScope);
        }

        public Task<Result<ClientScopeDetailsModel>> Get(ClientScopeEntity clientScope)
        {
            ClientScopeDetailsModel clientScopeDetailsModel = new ClientScopeDetailsModel(
                name: clientScope.Name,
                displayName: clientScope.DisplayName,
                description: clientScope.Description);

            return Task.FromResult(Result.Ok(clientScopeDetailsModel));
        }
    }
}
