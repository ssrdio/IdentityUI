using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models;
using SSRD.AdminUI.Template.Services;
using SSRD.CommonUtils.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.AdminUI.Template.Atttributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ValidateReCaptchaAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _action;
        public readonly double _requiredScore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiredScore">In range from 0.0 to 1.0</param>
        /// <param name="action"></param>
        public ValidateReCaptchaAttribute(double requiredScore = 0.5d, string action = null)
        {
            _action = action;
            _requiredScore = requiredScore;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ReCaptchaOptions reCaptchaOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<ReCaptchaOptions>>().Value;
            if (reCaptchaOptions.UseReCaptcha)
            {
                IReCaptchaRequest reCaptchaRequest = null;
                foreach (KeyValuePair<string, object> argument in context.ActionArguments)
                {
                    if(argument.Value is IReCaptchaRequest)
                    {
                        reCaptchaRequest = (IReCaptchaRequest)argument.Value;
                        break;
                    }
                }

                if (reCaptchaRequest == null)
                {
                    throw new Exception("recaptcha_token_not_foaund");
                }

                ReCaptchaTokentValidationService reCaptchaTokentValidationService =
                    context.HttpContext.RequestServices.GetRequiredService<ReCaptchaTokentValidationService>();

                Result result = await reCaptchaTokentValidationService.Validate(reCaptchaRequest.ReCaptchaToken, _requiredScore, _action);
                if (result.Failure)
                {
                    context.ModelState.AddResultErrors(result);
                }
            }

            await next.Invoke();
        }
    }
}
