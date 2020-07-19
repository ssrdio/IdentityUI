using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace IdentityUI.Sample.Sms.Controllers
{
    public class SendSmsReq
    {
        [Required]
        public string Number { get; set; }

        [Required]
        public string Message { get; set; }
    }

    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ISmsSender _smsSender;

        public HomeController(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        [HttpPost, ActionName("SendSms")]
        public async Task<IActionResult> SendSms([FromBody] SendSmsReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = await _smsSender.Send(req.Number, req.Message);
            if (result.Failure)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
