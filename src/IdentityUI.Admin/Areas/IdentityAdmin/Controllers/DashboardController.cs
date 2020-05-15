using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.User;
using SSRD.IdentityUI.Core.Data.Models.Constants;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    [Authorize(Roles = IdentityUIRoles.IDENTITY_MANAGMENT_ROLE)]
    [Route(PagePath.DASHBOARD)]
    public class DashboardController : BaseController
    {
        private readonly IUserDataService _userDataService;

        public DashboardController(IUserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            StatisticsViewModel viewModel = _userDataService.GetStatistics();

            return View(viewModel);
        }

        [HttpGet("GetRegistrationStatistics")]
        public IActionResult GetRegistrationStatistics([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<List<RegistrationsViewModel>> result = _userDataService.GetRegistrationStatistics(from, to);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }
    }
}
