using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Session;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    public class SessionController : BaseController
    {
        private readonly ISessionDataService _sessionDataService;
        private readonly ISessionService _sessionService;

        public SessionController(ISessionDataService sessionDataService, ISessionService sessionService)
        {
            _sessionDataService = sessionDataService;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<SessionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            Result<List<SessionModel>> result = await _sessionDataService.Get();

            return result.ToApiResult();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove([FromRoute] long id)
        {
            Result result = await _sessionService.Remove(id);

            return result.ToApiResult();
        }
    }
}
