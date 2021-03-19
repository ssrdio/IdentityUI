using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.OpenIdConnect.Mvc
{
    public class ClientController : BaseController
    {
        private readonly IClientDataService _clientDataService;

        public ClientController(IClientDataService clientDataService)
        {
            _clientDataService = clientDataService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            Result<ClientDetailsViewModel> result = await _clientDataService.GetDetailsViewModel(id);
            if(result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Credentials(string id)
        {
            Result<ClientMenuViewModel> result = await _clientDataService.GetMenuViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Scopes(string id)
        {
            Result<ClientMenuViewModel> result = await _clientDataService.GetMenuViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Consents(string id)
        {
            Result<ClientConsentViewModel> result = await _clientDataService.GetConsentViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Tokens(string id)
        {
            Result<ClientTokenViewModel> result = await _clientDataService.GetTokenViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }
}
