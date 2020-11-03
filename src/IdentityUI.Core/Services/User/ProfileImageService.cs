using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class ProfileImageService : IProfileImageService
    {
        private const string FAILED_TO_REMOVE_USER_IMAGE = "failed_to_remove_user_image";

        /// <summary>
        /// 0 = userId
        /// </summary>
        private const string URL_CACHE_KEY = "profile_image_url_{0}";

        /// <summary>
        /// 0 = userId
        /// </summary>
        private const string BLOBL_CACHE_KEY = "profile_image_blobl_{0}";

        private readonly string[] VALID_IMAGE_FORMATS = { ".JPG", ".PNG" };

        private readonly TimeSpan IMAGE_IN_CACHE_TIME_SPAN = new TimeSpan(1, 0, 0);

        private const string DEFAULT_PROFILE_IMAGE = "www.adminUI.template.vendors.fontawesome_free_5._14._0_web.svgs.solid.user-circle.svg";
        private const string DEFAULT_IMAGE_NAME = "fontawesome-free-5.14.0-web/svgs/solid/user-circle.svg";

        private readonly IBaseRepositoryAsync<UserImageEntity> _userImageRepository;
        private readonly IBaseDAO<UserImageEntity> _userImageDAO;

        private readonly IMemoryCache _memoryCache;
        private readonly IdentityUIOptions _identityUIOptions;

        private readonly ILogger<ProfileImageService> _logger;

        public ProfileImageService(
            IBaseRepositoryAsync<UserImageEntity> userImageRepository,
            IBaseDAO<UserImageEntity> userImageDAO,
            IMemoryCache memoryCache,
            IOptions<IdentityUIOptions> identityUIOptions,
            ILogger<ProfileImageService> logger)
        {
            _userImageRepository = userImageRepository;
            _userImageDAO = userImageDAO;

            _memoryCache = memoryCache;

            _identityUIOptions = identityUIOptions.Value;

            _logger = logger;
        }

        public async Task<Core.Models.Result.Result> UpdateProfileImage(string userId, UploadProfileImageRequest uploadProfileImageRequest)
        {
            if (uploadProfileImageRequest == null || uploadProfileImageRequest.File == null)
            {
                _logger.LogWarning($"No image. UserId {userId}");
                return Core.Models.Result.Result.Fail("no_profile_image", "No Image");
            }

            //TODO: changed how image format is validated
            string imageExtension = Path.GetExtension(uploadProfileImageRequest.File.FileName);
            if(!VALID_IMAGE_FORMATS.Contains(imageExtension.ToUpper()))
            {
                _logger.LogWarning($"Invalid image format. UserId {userId}, image extension {imageExtension}");
                return Core.Models.Result.Result.Fail("profile_image_invalid_format", "Image is invalid, please select '.JPG' or '.PNG' image format.");
            }

            if(uploadProfileImageRequest.File.Length > _identityUIOptions.MaxProfileImageSize)
            {
                _logger.LogWarning($"Image is to big. UserId {userId}, image size {uploadProfileImageRequest.File.Length}");
                return Core.Models.Result.Result.Fail("image_is_to_big", $"Image is to big. Max image size {_identityUIOptions.MaxProfileImageSize / 1024} kB");
            }

            if (uploadProfileImageRequest.File.FileName.Length > 250)
            {
                _logger.LogWarning($"Image name is to long. Image name length {uploadProfileImageRequest.File.FileName.Length}");
                return Core.Models.Result.Result.Fail("image_name_to_long", "Image name to long");
            }

            byte[] image;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await uploadProfileImageRequest.File.CopyToAsync(memoryStream);

                image = memoryStream.ToArray();
            }

            string urlCacheKey = string.Format(URL_CACHE_KEY, userId);
            string blobCacheKey = string.Format(BLOBL_CACHE_KEY, userId);
            _memoryCache.Remove(urlCacheKey);
            _memoryCache.Remove(blobCacheKey);

            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            UserImageEntity userImage = await _userImageRepository.SingleOrDefault(userImageSpecification);
            if (userImage == null)
            {
                UserImageEntity newUserImage = new UserImageEntity(
                    userId: userId,
                    blobImage: image,
                    fileName: uploadProfileImageRequest.File.FileName);

                bool addResult = await _userImageRepository.Add(newUserImage);
                if (!addResult)
                {
                    _logger.LogError($"Failed to add user image. UserId {userId}");
                    return Core.Models.Result.Result.Fail("failed_to_add_user_image", "Failed to add user image");
                }

                return Core.Models.Result.Result.Ok();
            }

            userImage.BlobImage = image;
            userImage.FileName = uploadProfileImageRequest.File.FileName;

            bool updateResult = await _userImageRepository.Update(userImage);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update user image. UserId {userId}");
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result<string>> GetProfileImageURL(string userId)
        {
            string cacheKey = string.Format(URL_CACHE_KEY, userId);

            if (_memoryCache.TryGetValue(cacheKey, out string imageUrl))
            {
                return Core.Models.Result.Result.Ok(imageUrl);
            }

            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            UserImageEntity userImage = await _userImageRepository.SingleOrDefault(userImageSpecification);
            if (userImage == null)
            {
                _logger.LogError($"No profile image. UserId {userId}");

                return Core.Models.Result.Result.Fail<string>("no_profile_image", "No Profile image");
            }

            _memoryCache.Set(cacheKey, userImage.URL, IMAGE_IN_CACHE_TIME_SPAN);

            return Core.Models.Result.Result.Ok(userImage.URL);
        }

        public async Task<Core.Models.Result.Result<FileData>> GetProfileImage(string userId)
        {
            string cacheKey = string.Format(BLOBL_CACHE_KEY, userId);

            if (_memoryCache.TryGetValue(cacheKey, out FileData image))
            {
                return Core.Models.Result.Result.Ok(image);
            }

            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            UserImageEntity userImage = await _userImageRepository.SingleOrDefault(userImageSpecification);
            if (userImage == null)
            {
                _logger.LogError($"No profile image. UserId {userId}");

                Core.Models.Result.Result<FileData> defaultImage = await GetDefaultImage();
                if(defaultImage.Failure)
                {
                    return Core.Models.Result.Result.Fail<FileData>(defaultImage.Errors);
                }

                _memoryCache.Set(cacheKey, defaultImage.Value, IMAGE_IN_CACHE_TIME_SPAN);

                return Core.Models.Result.Result.Ok(defaultImage.Value);
            }

            FileData fileData = new FileData(
                fileName: userImage.FileName,
                file: userImage.BlobImage);

            _memoryCache.Set(cacheKey, fileData, IMAGE_IN_CACHE_TIME_SPAN);

            return Core.Models.Result.Result.Ok(fileData);
        }

        private async Task<Core.Models.Result.Result<FileData>> GetDefaultImage()
        {
            Assembly assembly = typeof(SSRD.AdminUI.Template.IdentityManagementTemplateExtension).GetTypeInfo().Assembly;

            using (Stream resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{DEFAULT_PROFILE_IMAGE}"))
            {
                byte[] buffer = new byte[resource.Length];
                int read = await resource.ReadAsync(buffer, 0, (int)resource.Length);

                FileData fileData = new FileData(
                    fileName: DEFAULT_IMAGE_NAME,
                    file: buffer);

                return Core.Models.Result.Result.Ok(fileData);
            }
        }

        public async Task<Result> Remove(string userId)
        {
            IBaseSpecification<UserImageEntity, UserImageEntity> specification = SpecificationBuilder
                .Create<UserImageEntity>()
                .Where(x => x.UserId == userId)
                .Select(x => new UserImageEntity(
                    x.Id))
                .Build();

            UserImageEntity userImage = await _userImageDAO.SingleOrDefault(specification);
            if (userImage == null)
            {
                _logger.LogWarning($"User does not have a profile image. UserId {userId}");
                return Result.Ok();
            }

            bool removeResult = await _userImageDAO.Remove(userImage);
            if (!removeResult)
            {
                _logger.LogError($"Failed to remove user image");
                return Result.Fail(FAILED_TO_REMOVE_USER_IMAGE);
            }

            return Result.Ok();
        }
    }
}
