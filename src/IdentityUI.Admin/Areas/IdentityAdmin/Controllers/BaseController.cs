using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using System.Diagnostics;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Net.Mime;
using SSRD.CommonUtils.Result;
using System.Text;
using System.Text.Encodings.Web;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AuthorizeIdentityAdmin]
    [Area(PagePath.IDENTITY_ADMIN_AREA_NAME)]
    public class BaseController : Controller
    {
        public BaseController()
        {

        }

        [DebuggerStepThrough]
        protected string GetUserId()
        {
            //return (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected IActionResult NotFoundView()
        {
            return View("NotFound");
        }

        protected void SaveTempData<T>(string key, T data)
        {
            TempData[key] = JsonSerializer.Serialize(data);
        }

        protected T GetTempData<T>(string key)
        {
            bool exists = TempData.TryGetValue(key, out object obj);
            if(!exists)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>((string)obj);
        }

        protected async Task<IActionResult> JsonFile<T>(Result<T> result, string fileName)
        {
            if(result.Failure)
            {
                return result.ToApiResult();
            }

            MemoryStream memoryStream = new MemoryStream();
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            await JsonSerializer.SerializeAsync(memoryStream, result.Value, jsonSerializerOptions);

            //TODO: try to change that it works with stream
            return File(memoryStream.ToArray(), MediaTypeNames.Application.Json, fileName);
        }
    }
}
