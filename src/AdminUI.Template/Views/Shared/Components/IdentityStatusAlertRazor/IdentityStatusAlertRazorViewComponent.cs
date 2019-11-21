using SSRD.AdminUI.Template.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Views.Shared.Components.IdentityStatusAlertRazor
{
    public class IdentityStatusAlertRazorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(StatusAlertViewModel statusAlert)
        {
            return View(statusAlert);
        }
    }
}
