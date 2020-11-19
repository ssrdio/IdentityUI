using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
#if NET_CORE2
    [Produces("application/json")]
#elif NET_CORE3
    [Produces(MediaTypeNames.Application.Json)]
#endif
    [Route("api/[area]/{groupId}/[controller]/[action]")]
    public class GroupAdminApiBaseController : GroupAdminBaseController
    {
    }
}
