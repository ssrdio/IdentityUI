using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SSRD.AdminUI.Template.DependencyInjection;
using SSRD.AdminUI.Template.Models;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.AdminUI.Template.Services;

namespace SSRD.AdminUI.Template
{
    public static class IdentityManagementTemplateExtension
    {
        /// <summary>
        /// Registers services for AdminUI.Template
        /// </summary>
        /// <param name="services"></param>
        public static void AddAdminTemplate(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(UIConfigureOptions));
            services.Configure<AdminUIFaviconOptions>(options =>
            {
                options.FaviconUrl = "/adminUI/template/images/logo.png";
                options.FaviconType = "image/png";
            });

            services.AddScoped<ReCaptchaTokentValidationService>();

            services.AddHttpClient();

            services.AddSingleton<IValidator<Select2Request>, Select2RequestValidator>();
            services.AddSingleton<IValidator<DataTableRequest>, DataTableRequestValidator>();

            services.AddSingleton<IValidator<TimeRangeRequest>, TimeRangeRequestValidator>();
        }
    }
}
