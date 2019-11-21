using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.DependencyInjection
{
    public class IdentityUIAppBuilder
    {
        public IApplicationBuilder App { get; set; }

        public IdentityUIAppBuilder(IApplicationBuilder app)
        {
            App = app;
        }
    }
}
