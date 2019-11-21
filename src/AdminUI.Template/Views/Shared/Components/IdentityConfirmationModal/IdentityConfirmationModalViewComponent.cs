using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Views.Shared.Components.IdentityConfirmationModal
{
    public class IdentityConfirmationModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
