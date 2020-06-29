using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.AdminUI.Template.Models;
using Microsoft.AspNetCore.Mvc;

namespace SSRD.IdentityUI.Account.Areas.Account.Models
{
    public class StatusAlertViewExtension
    {
        public static StatusAlertViewModel Get(Result result)
        {
            if (!result.Failure)
            {
                return null;
            }

            List<string> messages = new List<string>();
            List<(string key, string value)> validationErrors = new List<(string key, string value)>();

            foreach (Result.ResultError error in result.Errors)
            {
                if (string.IsNullOrEmpty(error.PropertyName))
                {
                    messages.Add(error.Message);
                }
                else
                {
                    validationErrors.Add((error.PropertyName, error.Message));
                }
            }

            return new StatusAlertViewModel(
                messages: messages,
                validationErrors: validationErrors,
                success: false);
        }

        public static StatusAlertViewModel Get(string successMessage)
        {
            return new StatusAlertViewModel(
                message: successMessage,
                success: true);
        }
    }
}
