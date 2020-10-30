using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    [ApiExplorerSettings(IgnoreApi = false)]
#if NET_CORE2
    [Produces("application/json")]
#elif NET_CORE3
    [Produces(MediaTypeNames.Application.Json)]
#endif
    [Route("api/[area]/[controller]/[action]")]
    public class GroupAdminApiBaseController : GroupAdminBaseController
    {
    }
}
