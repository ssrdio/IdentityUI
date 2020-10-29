using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Setting;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Setting.Email;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Email.Models;
using SSRD.IdentityUI.Core.Services.Identity;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Setting
{
    [Route("/[area]/Setting/[controller]/[action]")]
    public class EmailController : BaseController
    {
        private readonly IEmailDataService _emailDataService;
        private readonly IManageEmailService _manageEmailService;

        public EmailController(IEmailDataService emailDataService, IManageEmailService manageEmailService)
        {
            _emailDataService = emailDataService;
            _manageEmailService = manageEmailService;
        }

        [Route("/[area]/Setting/[controller]")]
        public IActionResult Index()
        {
            EmailIndexViewModel emailIndexViewModel = _emailDataService.GetIndexViewModel();

            return View(emailIndexViewModel);
        }

        [Route("/[area]/Setting/[controller]/[action]/{id}")]
        public IActionResult Details(long id)
        {
            Result<EmailViewModel> result = _emailDataService.GetViewModel(id, User.GetUserId());
            if(result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<EmailTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] DataTableRequest dataTableRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<EmailTableModel>> result = _emailDataService.Get(dataTableRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public IActionResult Edit(long id, EditEmailRequest editEmail)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<EmailViewModel> role = _emailDataService.GetViewModel(id, GetUserId());
            if (role.Failure)
            {
                return NotFoundView();
            }

            Result editResult = _manageEmailService.Edit(id, editEmail);
            if (editResult.Failure)
            {
                ModelState.AddErrors(editResult.Errors);
                role.Value.StatusAlert = StatusAlertViewExtension.Get(editResult);

                return View("Details", role.Value);
            }

            role.Value.StatusAlert = StatusAlertViewExtension.Get("Email updated");
            return View("Details", role.Value);
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendTest([FromRoute] long id, [FromBody] SendTesEmailRequest sendTesEmail)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = await _manageEmailService.TestEmail(id, sendTesEmail);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
