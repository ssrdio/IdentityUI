using FluentValidation.Results;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Helper
{
    public static class ResultUtils
    {
        public static List<Result.ResultError> ToResultError(IList<ValidationFailure> errors)
        {
            return errors
                .Select(x => new Result.ResultError(
                    code: x.ErrorCode,
                    message: x.ErrorMessage,
                    propertyName: x.PropertyName))
                .ToList();
        }

        public static List<Result.ResultError> ToResultError(IEnumerable<IdentityError> errors)
        {
            return errors
                .Select(x => new Result.ResultError(
                    code: x.Code,
                    message: x.Description))
                .ToList();
        }


        public static List<Result.ResultError> ToResultError(IEnumerable<IdentityError> errors, string propertyName)
        {
            return errors
                .Select(x => new Result.ResultError(
                    code: x.Code,
                    message: x.Description,
                    propertyName: propertyName))
                .ToList();
        }

        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, List<Result.ResultError> errors)
        {
            foreach(Result.ResultError error in errors)
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

        public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, Result result, bool includePropertyNames = true)
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
    }
}
