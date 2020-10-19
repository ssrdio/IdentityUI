using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class AuditServiceBuilder
    {
        public IServiceCollection Services { get; }

        public AuditServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
