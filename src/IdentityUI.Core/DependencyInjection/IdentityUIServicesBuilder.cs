using System;
using System.Collections.Generic;
using System.Text;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace SSRD.IdentityUI.Core.DependencyInjection
{
    public class IdentityUIServicesBuilder
    {
        public IServiceCollection Services { get; }

        public IdentityUIEndpoints IdentityManagementEndpoints {get; }

        public IConfiguration Configuration { get; set; }

        public IdentityUIServicesBuilder(IServiceCollection services, IdentityUIEndpoints identityManagementEndpoints, IConfiguration configuration)
        {
            Services = services;
            IdentityManagementEndpoints = identityManagementEndpoints;
            Configuration = configuration;
        }
    }
}
