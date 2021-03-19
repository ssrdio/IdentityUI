using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SSRD.CommonUtils.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ValidateModelAttribute : Attribute, IActionFilter
    {
        private const string MODEL_VALIDATED_KEY = "model_validated_key";

        public ValidateModelAttribute()
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Items.ContainsKey(MODEL_VALIDATED_KEY))
            {
                return;
            }

            context.HttpContext.Items.Add(MODEL_VALIDATED_KEY, MODEL_VALIDATED_KEY);

            bool controllerHasSkip = context.Controller.GetType().CustomAttributes
                .Where(x => x.AttributeType == typeof(SkipModelValidationAttribute))
                .Any();

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            bool actionHasValidate = controllerActionDescriptor?.MethodInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(SkipModelValidationAttribute))
                .Any() ?? false;

            bool actionHasSkip = controllerActionDescriptor?.MethodInfo.CustomAttributes
                .Where(x => x.AttributeType == typeof(SkipModelValidationAttribute))
                .Any() ?? false;

            ILogger<ValidateModelAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ValidateModelAttribute>>();

            foreach (KeyValuePair<string, object> keyValue in context.ActionArguments)
            {
                Type argumentType = keyValue.Value.GetType();

                if (!IsValidationTypeType(argumentType))
                {
                    continue;
                }

                ControllerParameterDescriptor controllerParameter = context.ActionDescriptor.Parameters
                    .Where(x => x.Name == keyValue.Key)
                    .Select(x => x as ControllerParameterDescriptor)
                    .Single();

                if(controllerHasSkip && !actionHasValidate)
                {
                    bool parameterHasValidate = controllerParameter.ParameterInfo.CustomAttributes
                        .Where(x => x.AttributeType == typeof(ValidateModelAttribute))
                        .Any();
                    if(!parameterHasValidate)
                    {
                        continue;
                    }
                }

                bool parameterHasSkip = controllerParameter.ParameterInfo.CustomAttributes
                    .Where(x => x.AttributeType == typeof(SkipModelValidationAttribute))
                    .Any();

                ValidationRuleSetAttribute validationRuleSet = controllerParameter.ParameterInfo.GetCustomAttribute(typeof(ValidationRuleSetAttribute)) as ValidationRuleSetAttribute;

                Type type = typeof(IValidator<>).MakeGenericType(argumentType);

                IValidator validator = (IValidator)context.HttpContext.RequestServices.GetRequiredService(type);

                ValidationResult validationResult;

                if (validationRuleSet != null && validationRuleSet.RuleSets != null)
                {
                    IValidatorSelector validationSelector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(validationRuleSet.RuleSets);
                    ValidationContext validationContext = new ValidationContext(keyValue.Value, new PropertyChain(), validationSelector);

                    validationResult = validator.Validate(validationContext);
                }
                else
                {
                    validationResult = validator.Validate(keyValue.Value);
                }

                if (!validationResult.IsValid)
                {
                    Result.Result result = Result.Result.Fail(validationResult.ToResultError());

                    context.ModelState.AddResultErrors(result);
                }
            }

            if (context.ModelState.IsValid)
            {
                return;
            }

            logger.LogTrace($"Invalid model state. Resource: {context.HttpContext.Request.Path} Errors: {JsonSerializer.Serialize(context.ModelState.Values)}");

            context.Result = new BadRequestObjectResult(context.ModelState);
        }

        private bool IsValidationTypeType(Type type)
        {
            if (type.IsPrimitive
                || type.IsEnum
                || !type.IsClass
                || type == typeof(string)
                || type == typeof(decimal))
            {
                return false;
            }

            //TODO: check nullable

            return true;
        }
    }
}
