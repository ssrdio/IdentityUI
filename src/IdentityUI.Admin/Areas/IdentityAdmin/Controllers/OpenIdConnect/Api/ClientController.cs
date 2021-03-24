using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Validation;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.OpenIdConnect.Api
{
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ClientController : BaseApiController
    {
        private readonly IClientDataService _clientDataService;
        private readonly IAddClientService _addClientService;
        private readonly IManageClientService _manageClientService;
        private readonly IClientConsentService _clientConsentService;
        private readonly IClientTokenService _clientTokenService;
        public ClientController(
            IClientDataService clientDataService,
            IAddClientService addClientService,
            IManageClientService manageClientService,
            IClientConsentService clientConsentService,
            IClientTokenService clientTokenService)
        {
            _clientDataService = clientDataService;
            _addClientService = addClientService;
            _manageClientService = manageClientService;
            _clientConsentService = clientConsentService;
            _clientTokenService = clientTokenService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<ClientTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<ClientTableModel>> result = await _clientDataService.Get(dataTableRequest);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDetailsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            Result<ClientDetailsModel> result = await _clientDataService.Get(id);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddSinglePageClient([FromBody] AddSinglePageClientModel addSinglePageClientModel)
        {
            Result<IdStringModel> result = await _addClientService.AddSinglePageClient(addSinglePageClientModel);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddWebAppClient([FromBody] AddWebAppClientModel addServerSideClientModel)
        {
            Result<IdStringModel> result = await _addClientService.AddWebAppClient(addServerSideClientModel);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddClientCredentialsClient([FromBody] AddClientCredentialsClientModel addClientCredentialsClientModel)
        {
            Result<IdStringModel> result = await _addClientService.AddClientCredentials(addClientCredentialsClientModel);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMobileClient([FromBody] AddMobileClientModel addMobileClientModel)
        {
            Result<IdStringModel> result = await _addClientService.AddMobileClient(addMobileClientModel);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddCustomClient([FromBody] AddCustomClientModel addCustomClientModel)
        {
            Result<IdStringModel> result = await _addClientService.AddCustomClient(addCustomClientModel);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateClientModel updateClientModel)
        {
            Result<ClientEntity> updateResult = await _manageClientService.Update(id, updateClientModel);
            if (updateResult.Failure)
            {
                return updateResult.ToApiResult();
            }

            Result<ClientDetailsModel> getResult = await _clientDataService.Get(id);

            return getResult.ToApiResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            Result result = await _manageClientService.Remove(id);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientIdModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientId([FromRoute] string id)
        {
            Result<ClientIdModel> result = await _clientDataService.GetClientId(id);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClientId([FromRoute] string id, [FromBody] UpdateClientIdModel updateClientIdModel)
        {
            Result result = await _manageClientService.UpdateClientId(id, updateClientIdModel);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientIdModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientSecret([FromRoute] string id)
        {
            Result<ClientSecretModel> result = await _clientDataService.GetClientSecret(id);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateNewClientSecret([FromRoute] string id)
        {
            Result<GenerateNewClientSecretModel> result = await _manageClientService.GenerateNewClientSecret(id);

            return result.ToApiResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteClientSecret([FromRoute] string id)
        {
            Result result = await _manageClientService.RemoveClientSecret(id);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientScopesModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScopes([FromRoute] string id)
        {
            Result<ClientScopesModel> result = await _clientDataService.GetScopes(id);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddScopes([FromRoute] string id, [FromBody] ManageClientScopesModel clientScopes)
        {
            Result result = await _manageClientService.AddScopes(id, clientScopes);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveScopes([FromRoute] string id, [FromBody] ManageClientScopesModel clientScopes)
        {
            Result result = await _manageClientService.RemoveScopes(id, clientScopes);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DataTableResult<ClientConsentTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConsents(
            [FromRoute] string id,
            [FromQuery][ValidationRuleSet("allowAll")] DataTableRequest dataTableRequest,
            [FromQuery] ClientConsentTableFilterModel clientConsentTableFilterModel)
        {
            Result<DataTableResult<ClientConsentTableModel>> result = await _clientDataService.GetConsents(id, dataTableRequest, clientConsentTableFilterModel);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConsentSubjects([FromRoute] string id, [FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _clientDataService.GetConsentSubjects(id, select2Request);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenSubjects([FromRoute] string id, [FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _clientDataService.GetTokenSubjects(id, select2Request);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DataTableResult<ClientTokenTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokens(
            [FromRoute] string id,
            [FromQuery][ValidationRuleSet("allowAll")] DataTableRequest dataTableRequest,
            [FromQuery] ClientTokenTableFilterModel clientTokenTableFilter)
        {
            Result<DataTableResult<ClientTokenTableModel>> result = await _clientDataService.GetTokens(id, dataTableRequest, clientTokenTableFilter);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateConsents([FromRoute] string id, [FromBody] List<string> consentIds)
        {
            Result result = await _clientConsentService.SetValidStatus(consentIds);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeConsents([FromRoute] string id, [FromBody] List<string> consentIds)
        {
            Result result = await _clientConsentService.SetRevokedStatus(consentIds);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeTokens([FromRoute] string id, [FromBody] List<string> tokenIds)
        {
            Result result = await _clientTokenService.RevokeTokens(tokenIds);

            return result.ToApiResult();
        }
    }
}
