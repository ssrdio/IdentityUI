using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SSRD.Audit.Data;
using SSRD.Audit.Models;
using SSRD.Audit.Services;
using System;

namespace SSRD.Audit.Extensions
{
    public static class AuditExtensions
    {
        public static AuditServiceBuilder AddAudit(this IServiceCollection services)
        {
            services.AddScoped<IAuditLogger, AuditDbLogger>();

            services.AddScoped<IAuditSubjectDataService>(x =>
            {
                IOptions<AuditOptions> auditOptions = x.GetRequiredService<IOptions<AuditOptions>>();

                IHttpContextAccessor httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
                if(httpContextAccessor.HttpContext != null)
                {
                    return new HttpContextAuditDataService(httpContextAccessor, auditOptions);
                }

                IBackgroundServiceContextAccessor backgroundServiceContextAccessor = x.GetRequiredService<IBackgroundServiceContextAccessor>();
                if(backgroundServiceContextAccessor.BackgroundServiceContext != null)
                {
                    return new BackgroundServiceAuditSubjectDataService(backgroundServiceContextAccessor);
                }

                return new DefaultAuditSubjectService(auditOptions);
            });

            services.AddSingleton<IBackgroundServiceContextAccessor, BackgroundServiceContextAccessor>();

            services.Configure<AuditOptions>(x => new AuditOptions());

            return new AuditServiceBuilder(services);
        }

        public static AuditServiceBuilder AddAuditDatabase(this AuditServiceBuilder builder, Action<DbContextOptionsBuilder> dbContextOptionAction)
        {
            builder.Services.AddDbContext<AuditDbContext>(dbContextOptionAction);

            return builder;
        }
    }
}
