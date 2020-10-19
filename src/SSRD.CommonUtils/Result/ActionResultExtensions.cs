using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SSRD.CommonUtils.Result
{
    public static class ActionResultExtensions
    {
        public static IActionResult ToApiResult(this Result result, bool includePropertyNames = true)
        {
            return ToApiResult(result, null, includePropertyNames);
        }

        public static IActionResult ToApiResult<T>(this Result<T> result, bool includePropertyNames = true)
        {
            return ToApiResult(result, result.Value, includePropertyNames);
        }

        private static IActionResult ToApiResult(Result result, object model, bool includePropertyNames)
        {
            if (result.Failure)
            {
                ModelStateDictionary modelState = new ModelStateDictionary();
                modelState.AddResultErrors(result, includePropertyNames);

                BadRequestObjectResult badRequestObjectResult = new BadRequestObjectResult(modelState);

                return badRequestObjectResult;
            }
            else
            {
                OkObjectResult okObjectResult;

                if (model == null)
                {
                    okObjectResult = new OkObjectResult(new EmptyResult());
                }
                else
                {
                    okObjectResult = new OkObjectResult(model);
                }

                return okObjectResult;
            }
        }
    }
}
