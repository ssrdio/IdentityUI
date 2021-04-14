using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Consent;
using SSRD.IdentityUI.Core.Interfaces.Services.OpenIdConnect;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    [Route("/api/[area]/[controller]/[action]")]
    public class ConsentController : BaseController
    {
        private readonly IConsentDataService _consentDataService;
        private readonly IClientConsentService _clientConsentService;

        public ConsentController(IConsentDataService consentDataService, IClientConsentService clientConsentService)
        {
            _consentDataService = consentDataService;
            _clientConsentService = clientConsentService;
        }

        [HttpGet("/[area]/[controller]")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<ConsentModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<ConsentModel>> result = await _consentDataService.Get(dataTableRequest);

            return result.ToApiResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove([FromRoute] string id)
        {
            Result result = await _clientConsentService.RevokeConsent(id);

            return result.ToApiResult();
        }
    }
}
