using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Session;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    [Route("/api/[area]/[controller]/[action]")]
    public class SessionController : BaseController
    {
        private readonly ISessionDataService _sessionDataService;
        private readonly ISessionService _sessionService;

        public SessionController(ISessionDataService sessionDataService, ISessionService sessionService)
        {
            _sessionDataService = sessionDataService;
            _sessionService = sessionService;
        }

        [HttpGet("/[area]/[controller]")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<SessionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(DataTableRequest dataTableRequest)
        {
            Result<DataTableResult<SessionModel>> result = await _sessionDataService.Get(dataTableRequest);

            return result.ToApiResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove([FromRoute] long id)
        {
            Result result = await _sessionService.Remove(id);

            return result.ToApiResult();
        }
    }
}
