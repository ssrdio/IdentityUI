using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Models
{
    public class StatusAlertViewModel
    {
        public List<string> Messages { get; set; }
        public List<(string key, string value)> ValidationErrors { get; set; }
        public bool Success { get; set; }

        public StatusAlertViewModel()
        {
        }

        public StatusAlertViewModel(string message, bool success)
        {
            Messages = new List<string> { message };
            Success = success;
        }

        public StatusAlertViewModel(List<string> messages, bool success)
        {
            Messages = messages;
            Success = success;
        }

        public StatusAlertViewModel(List<string> messages, List<(string key, string value)> validationErrors, bool success)
        {
            Messages = messages;
            ValidationErrors = validationErrors;
            Success = success;
        }
    }
}
