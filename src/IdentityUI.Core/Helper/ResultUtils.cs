using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SSRD.CommonUtils.Result;
using System.Collections.Generic;
using System.Linq;

namespace SSRD.IdentityUI.Core.Helper
{
    public static class ResultUtils
    {
        public static List<Models.Result.Result.ResultError> ToResultError(IList<ValidationFailure> errors)
        {
            return errors
                .Select(x => new Models.Result.Result.ResultError(
                    code: x.ErrorCode,
                    message: x.ErrorMessage,
                    propertyName: x.PropertyName))
                .ToList();
        }

        public static List<Models.Result.Result.ResultError> ToResultError(IEnumerable<IdentityError> errors)
        {
            return errors
                .Select(x => new Models.Result.Result.ResultError(
                    code: x.Code,
                    message: x.Description))
                .ToList();
        }


        public static List<Models.Result.Result.ResultError> ToResultError(IEnumerable<IdentityError> errors, string propertyName)
        {
            return errors
                .Select(x => new Models.Result.Result.ResultError(
                    code: x.Code,
                    message: x.Description,
                    propertyName: propertyName))
                .ToList();
        }

        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, List<Models.Result.Result.ResultError> errors)
        {
            foreach(Models.Result.Result.ResultError error in errors)
            {
                if(!string.IsNullOrEmpty(error.PropertyName))
                {
                    modelState.AddModelError(error.PropertyName, error.Message);
                }
                else
                {
                    modelState.AddModelError(string.Empty, error.Message);
                }
            }

            return modelState;
        }

        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, List<(string key, string value)> errors)
        {
            if(errors == null)
            {
                return modelState;
            }

            foreach ((string key, string value) in errors)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    modelState.AddModelError(key, value);
                }
                else
                {
                    modelState.AddModelError(string.Empty, value);
                }
            }

            return modelState;
        }

        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, Models.Result.Result result, bool includePropertyNames = true)
        {
            if (!result.Failure)
            {
                return modelState;
            }

            foreach (var error in result.Errors)
            {
                if (!string.IsNullOrEmpty(error.PropertyName) && includePropertyNames)
                {
                    modelState.AddModelError(error.PropertyName, error.Message);
                }
                else
                {
                    modelState.AddModelError(string.Empty, error.Message);
                }
            }

            return modelState;
        }

        public static List<ResultMessage> ToResultError(this IdentityResult identityResult)
        {
            return identityResult.Errors
                .Select(x => new ResultMessage(
                    code: x.Code,
                    level: ResultMessageLevels.Error))
                .ToList();
        }

        public static Result ToNewResult(this Core.Models.Result.Result result)
        {
            if(result.Failure)
            {
                List<ResultMessage> newResultMessages = new List<ResultMessage>();

                foreach(Models.Result.Result.ResultError oldResult in result.Errors)
                {
                    if (!string.IsNullOrEmpty(oldResult.PropertyName))
                    {
                        ResultMessage resultMessage = new PropertyResultMessage(
                            code: oldResult.Message,
                            level: ResultMessageLevels.Error,
                            propertyName: oldResult.PropertyName);

                        newResultMessages.Add(resultMessage);
                    }
                    else
                    {
                        ResultMessage resultMessage = new ResultMessage(
                            code: oldResult.Code,
                            level: ResultMessageLevels.Error);

                        newResultMessages.Add(resultMessage);
                    }
                }

                return Result.Fail(newResultMessages);
            }

            return Result.Ok();
        }

        public static Core.Models.Result.Result ToOldResult(this Result result)
        {
            if(result.Failure)
            {
                return Core.Models.Result.Result.Fail(result.ResultMessages.Select(x => new Core.Models.Result.Result.ResultError(x.Code, x.Code, null)).ToList());
            }

            return Core.Models.Result.Result.Ok();
        }

        public static Result<T> ToNewResult<T>(this Core.Models.Result.Result<T> result)
        {
            if (result.Failure)
            {
                return Result.Fail<T>(result.Errors.Select(x => new ResultMessage(x.Code, ResultMessageLevels.Error)));
            }

            return Result.Ok(result.Value);
        }

        public static Core.Models.Result.Result<T> ToOldResult<T>(this Result<T> result)
        {
            if (result.Failure)
            {
                return Core.Models.Result.Result.Fail<T>(result.ResultMessages.Select(x => new Core.Models.Result.Result.ResultError(x.Code, x.Code, null)).ToList());
            }

            return Core.Models.Result.Result.Ok(result.Value);
        }
    }
}
