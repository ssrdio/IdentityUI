using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.OpenIdConnect.Mvc
{
    public class ClientScopeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string id)
        {
            ViewBag.Id = id;

            return View();
        }
    }
}
