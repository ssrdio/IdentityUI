using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace SSRD.CommonUtils.Result
{
    public static class ResultExtensions
    {
        public static List<PropertyResultMessage> ToResultError(this FluentValidation.Results.ValidationResult validationResult)
        {
            return validationResult.Errors
                .Select(x => new PropertyResultMessage(
                    code: x.ErrorMessage,
                    propertyName: x.PropertyName,
                    level: ResultMessageLevels.Error))
                .ToList();
        }

        public static ResultMessage ToResultError(this System.ComponentModel.DataAnnotations.ValidationResult validationResult)
        {
            ResultMessage propertyResultMessage = new ResultMessage(
                code: validationResult.ErrorMessage,
                level: ResultMessageLevels.Error);

            return propertyResultMessage;
        }

        public static List<ResultMessage> ToResultError(this IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
        {
            return validationResults
                .Select(x => x.ToResultError())
                .ToList();
        }

        public static ModelStateDictionary AddResultErrors(this ModelStateDictionary modelState, Result result, bool includePropertyNames = true)
        {
            IEnumerable<ResultMessage> messages = result.ResultMessages
                .Where(x => x.Level == ResultMessageLevels.Error);

            foreach (ResultMessage message in messages)
            {
                if(message is PropertyResultMessage propertyResultMessage && includePropertyNames)
                {
                    modelState.AddModelError(propertyResultMessage.PropertyName, propertyResultMessage.ToMessage());
                }
                else
                {
                    modelState.AddModelError(string.Empty, message.ToMessage());
                }
            }

            return modelState;
        }
    }
}
