using Microsoft.Extensions.Logging;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class IdentityUIDefaultProfileImageService : IDefaultProfileImageService
    {
        private const string PROFILE_IMAGE_NOT_FOUND = "profile_image_not_found";

        private const string DEFAULT_PROFILE_IMAGE = "www.adminUI.template.vendors.fontawesome_free_5._14._0_web.svgs.solid.user-circle.svg";
        private const string DEFAULT_IMAGE_NAME = "fontawesome-free-5.14.0-web/svgs/solid/user-circle.svg";

        private readonly ILogger<IdentityUIDefaultProfileImageService> _logger;

        public IdentityUIDefaultProfileImageService(ILogger<IdentityUIDefaultProfileImageService> logger)
        {
            _logger = logger;
        }

        public async Task<Result<FileData>> Get()
        {
            Assembly assembly = typeof(SSRD.AdminUI.Template.IdentityManagementTemplateExtension).GetTypeInfo().Assembly;

            using (Stream resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{DEFAULT_PROFILE_IMAGE}"))
            {
                byte[] buffer = new byte[resource.Length];
                try
                {
                    int read = await resource.ReadAsync(buffer, 0, (int)resource.Length);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Failed to get default profile image");
                    return Result.Fail<FileData>(PROFILE_IMAGE_NOT_FOUND);
                }

                FileData fileData = new FileData(
                    fileName: DEFAULT_IMAGE_NAME,
                    file: buffer);

                return Result.Ok(fileData);
            }
        }
    }
}
