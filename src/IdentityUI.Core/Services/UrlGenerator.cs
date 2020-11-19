using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SSRD.IdentityUI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services
{
    internal class UrlGenerator : IUrlGenerator
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlGenerator(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);

            _httpContextAccessor = httpContextAccessor;
        }

        [Obsolete("This function does not work properly if you use proxy or load balancer")]
        public string GenerateActionUrl(string action, string controller, object values)
        {
            //TODO: fix this so it uses BaseURL or looks in headers for host 

            return _urlHelper.Action(action, controller, values, _httpContextAccessor.HttpContext.Request.Scheme, _httpContextAccessor.HttpContext.Request.Host.ToString());
        }
    }
}
