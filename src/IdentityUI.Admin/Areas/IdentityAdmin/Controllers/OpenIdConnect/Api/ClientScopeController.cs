using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using SSRD.IdentityUI.Core.Services.OpenIdConnect.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.OpenIdConnect.Api
{
    public class ClientScopeController : BaseApiController
    {
        private readonly IClientScopeDataService _clientScopeDataService;
        private readonly IClientScopeService _clientScopeService;

        public ClientScopeController(IClientScopeDataService clientScopeDataService, IClientScopeService clientScopeService)
        {
            _clientScopeDataService = clientScopeDataService;
            _clientScopeService = clientScopeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<ClientScopeTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<ClientScopeTableModel>> result = await _clientScopeDataService.Get(dataTableRequest);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientScopeDetailsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            Result<ClientScopeDetailsModel> result = await _clientScopeDataService.Get(id);

            return result.ToApiResult();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromBody] AddClientScopeModel addClientScopeModel)
        {
            Result result = await _clientScopeService.Add(addClientScopeModel);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(ClientScopeDetailsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateClientScopeModel updateClientScopeModel)
        {
            Result<ClientScopeEntity> updateResult = await _clientScopeService.Update(id, updateClientScopeModel);
            if(updateResult.Failure)
            {
                return updateResult.ToApiResult();
            }

            Result<ClientScopeDetailsModel> getResult = await _clientScopeDataService.Get(updateResult.Value);

            return getResult.ToApiResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            Result result = await _clientScopeService.Remove(id);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSelect([FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _clientScopeDataService.Get(select2Request);

            return result.ToApiResult();
        }
    }
}
