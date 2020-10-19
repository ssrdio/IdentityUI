using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SSRD.Audit.Attributes;
using SSRD.Audit.Data;
using SSRD.Audit.Models;
using SSRD.Audit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.Audit.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public sealed class AuditAttribute : Attribute, IAsyncActionFilter
    {
        private const string AUDIT_PROCCESSINT_KEY = "audit_proccessing";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.HttpContext.Items.TryGetValue(AUDIT_PROCCESSINT_KEY, out object auditProccessing))
            {
                await next();

                return;
            }

            context.HttpContext.Items.Add(AUDIT_PROCCESSINT_KEY, null);

            ActionExecutedContext resultContext = await next();

            ControllerActionDescriptor actionDescriptor = ((ControllerActionDescriptor)context.ActionDescriptor);
            if(actionDescriptor.MethodInfo.GetCustomAttributes(true).Any(x => x.GetType() == typeof(AuditIgnoreAttribute)))
            {
                return;
            }

            if(!actionDescriptor.MethodInfo.CustomAttributes.Any(x => x.AttributeType == typeof(AuditAttribute)) 
                && actionDescriptor.ControllerTypeInfo.GetCustomAttributes(true).Any(x => x.GetType() == typeof(AuditIgnoreAttribute)))
            {
                return;
            }

            IAuditLogger auditLogger = context.HttpContext.RequestServices.GetRequiredService<IAuditLogger>();
            AuditOptions auditOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AuditOptions>>().Value;

            KeyValuePair<string, object> objectIdentifierResult;

            AuditObjectIdentifierKey objectIdentitiferKey = actionDescriptor.MethodInfo.GetCustomAttribute<AuditObjectIdentifierKey>();
            if(objectIdentitiferKey != null)
            {
                objectIdentifierResult = resultContext.RouteData.Values
                    .Where(x => x.Key == objectIdentitiferKey.ObjectKey)
                    .SingleOrDefault();
            }
            else
            {
                objectIdentifierResult = resultContext.RouteData.Values
                    .Where(x => x.Key != "area")
                    .Where(x => x.Key != "controller")
                    .Where(x => x.Key != "action")
                    .LastOrDefault();
            }

            string objectIdentifier = null;
            if (objectIdentifierResult.Value != null)
            {
                objectIdentifier = objectIdentifierResult.Value.ToString();
            }      

            switch (resultContext.Result)
            {
                case OkObjectResult _:
                    {
                        OkObjectResult okObjectResult = (OkObjectResult)resultContext.Result;

                        AuditObjectData auditObject = new AuditObjectData(
                            actionType: ActionTypes.Get,
                            objectType: okObjectResult.Value.GetType().GetNameWithGeneric(),
                            objectIdentifier: objectIdentifier,
                            objectMetadata: JsonConvert.SerializeObject(okObjectResult.Value));

                        await auditLogger.LogAsync(auditObject);
                        break;
                    }

                case ViewResult _:
                    {
                        ViewResult viewResult = (ViewResult)resultContext.Result;

                        List<AuditObjectData> auditObjects = new List<AuditObjectData>();

                        if (viewResult.Model != null)
                        {
                            AuditObjectData auditObject = new AuditObjectData(
                                actionType: ActionTypes.Get,
                                objectType: viewResult.GetType().Name,
                                objectIdentifier: objectIdentifier,
                                objectMetadata: JsonConvert.SerializeObject(viewResult.Model));

                            auditObjects.Add(auditObject);
                        }

                        if (auditOptions.AuditViewData && viewResult.ViewData.Any())
                        {
                            AuditObjectData auditObject = new AuditObjectData(
                                actionType: ActionTypes.Get,
                                objectType: viewResult.ViewData.GetType().Name,
                                objectIdentifier: objectIdentifier,
                                objectMetadata: JsonConvert.SerializeObject(viewResult.ViewData));

                            auditObjects.Add(auditObject);
                        }

                        await auditLogger.LogAsync(auditObjects);
                        break;
                    }
                case BadRequestObjectResult _:
                    {
                        if(!auditOptions.AuditBadRequest)
                        {
                            break;
                        }

                        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)resultContext.Result;

                        AuditObjectData auditObject = new AuditObjectData(
                            actionType: ActionTypes.BadRequest,
                            objectType: badRequestObjectResult.Value.GetType().Name,
                            objectIdentifier: objectIdentifier,
                            objectMetadata: JsonConvert.SerializeObject(badRequestObjectResult.Value));

                        await auditLogger.LogAsync(auditObject);

                        break;
                    }
                case FileContentResult _:
                    {
                        FileContentResult fileContentResult = (FileContentResult)resultContext.Result;

                        AuditObjectData auditObjectData = new AuditObjectData(
                            actionType: ActionTypes.Get,
                            objectType: resultContext.Result.GetType().Name,
                            objectIdentifier: objectIdentifier,
                            objectMetadata: JsonConvert.SerializeObject(new { fileContentResult.ContentType, fileContentResult.FileDownloadName }));

                        await auditLogger.LogAsync(auditObjectData);

                        break;
                    }
                case RedirectResult _:
                case RedirectToActionResult _:
                case RedirectToPageResult _:
                case RedirectToRouteResult _:
                case LocalRedirectResult _:
                    {
                        break;
                    }
                default:
                    {
                        if (resultContext.Result != null)
                        {
                            AuditObjectData auditObject = new AuditObjectData(
                                actionType: ActionTypes.Get,
                                objectType: resultContext.Result.GetType().Name,
                                objectIdentifier: objectIdentifier,
                                objectMetadata: null);

                            await auditLogger.LogAsync(auditObject);
                        }

                        break;
                    }
            }
        }
    }

    internal static class TypeExtensions
    {
        public static string GetNameWithGeneric(this Type type)
        {
            StringBuilder stringBuilder = new StringBuilder(type.Name);

            if(type.IsGenericType)
            {
                stringBuilder.Append("<");

                Type[] generics = type.GetGenericArguments();
                for(int i = 0; i < generics.Length; i++)
                {
                    if(i != 0)
                    {
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append(generics[i].Name);
                }

                stringBuilder.Append(">");
            }

            return stringBuilder.ToString();
        }
    }
}
