using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Views.Shared.Components.AdminUIErrorAlert
{
    public class AdminUIErrorAlertViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}
