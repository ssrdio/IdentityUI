using System;
using System.Collections.Generic;
using System.Text;
using SSRD.IdentityUI.Core.Models.Options;
using Microsoft.Extensions.DependencyInjection;

namespace SSRD.IdentityUI.Core.DependencyInjection
{
    public class IdentityUIServicesBuilder
    {
        public IServiceCollection Services { get; }

        public IdentityUIEndpoints IdentityManagementEndpoints {get; }

        public IdentityUIServicesBuilder(IServiceCollection services, IdentityUIEndpoints identityManagementEndpoints)
        {
            Services = services;
            IdentityManagementEndpoints = identityManagementEndpoints;
        }
    }
}
