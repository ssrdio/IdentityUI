using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Validation;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    [AuthorizeIdentityAdmin]
    [ValidateModel]
    [Area(PagePath.IDENTITY_ADMIN_AREA_NAME)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
#if NET_CORE2
    [Produces("application/json")]
#else
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
#endif
    [Route("api/[area]/[controller]/[action]")]
    public class BaseApiController : ControllerBase
    {
    }
}
