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
        public DatabaseOptions DatabaseOptions { get; set; }
        public IdentityUIOptions IdentityUIOptions { get; set; }

        public IConfiguration Configuration { get; set; }

        public IdentityUIServicesBuilder(
            IServiceCollection services,
            IdentityUIEndpoints identityManagementEndpoints,
            DatabaseOptions databaseOptions,
            IdentityUIOptions identityUIOptions,
            IConfiguration configuration)
        {
            Services = services;
            IdentityManagementEndpoints = identityManagementEndpoints;
            DatabaseOptions = databaseOptions;
            IdentityUIOptions = identityUIOptions;
            Configuration = configuration;
        }
    }
}
