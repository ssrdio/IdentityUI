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

            List<string> messages = result.Errors
                 .Where(x => x.PropertyName == null)
                 .Select(x => x.Message)
                 .ToList();

            if(messages.Count == 0)
            {
                return null;
            }

            return new StatusAlertViewModel(
                messages: messages,
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
