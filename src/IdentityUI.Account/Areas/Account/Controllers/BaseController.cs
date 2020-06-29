using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using Newtonsoft.Json;
using SSRD.IdentityUI.Core.Services.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Area(PagePath.ACCOUNT_AREA_NAME)]
    public class BaseController : Controller
    {
        [DebuggerStepThrough]
        protected string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [DebuggerStepThrough]
        protected string GetSessionCode()
        {
            return User.FindFirstValue(IdentityUIClaims.SESSION_CODE);
        }

        [DebuggerStepThrough]
        protected string GetIp()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        protected void SaveTempData<T>(string key, T data)
        {
            TempData[key] = JsonConvert.SerializeObject(data);
        }

        protected T GetTempData<T>(string key)
        {
            bool exists = TempData.TryGetValue(key, out object obj);
            if (!exists)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>((string)obj);
        }
    }
}
