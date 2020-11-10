using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces;
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
        private readonly string PROFILE_IMAGE_NOT_FOUND = "profile_image_not_found";

        private readonly string INVALID_PROFILE_IMAGE_FORMAT = "invalid_profile_image_format";
        private readonly string PROFILE_IMAGE_TOO_BIG = "profile_image_too_big";
        private readonly string PROFILE_IMAGE_NAME_TOO_LONG = "profile_image_name_too_long";

        private readonly string FAILED_TO_ADD_PROFILE_IMAGE = "failed_to_add_profile_image";
        private readonly string FAILED_TO_UPDATE_PROFILE_IMAGE = "failed_to_update_profile_image";

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

        private readonly IBaseDAO<UserImageEntity> _userImageDAO;

        private readonly IMemoryCache _memoryCache;
        private readonly IDefaultProfileImageService _defaultProfileImageService;

        private readonly IdentityUIOptions _identityUIOptions;

        private readonly ILogger<ProfileImageService> _logger;

        public ProfileImageService(
            IBaseDAO<UserImageEntity> userImageDAO,
            IMemoryCache memoryCache,
            IDefaultProfileImageService defaultProfileImageService,
            IOptions<IdentityUIOptions> identityUIOptions,
            ILogger<ProfileImageService> logger)
        {
            _userImageDAO = userImageDAO;

            _memoryCache = memoryCache;
            _defaultProfileImageService = defaultProfileImageService;

            _identityUIOptions = identityUIOptions.Value;

            _logger = logger;
        }

        public async Task<Core.Models.Result.Result> UpdateProfileImage(string userId, UploadProfileImageRequest uploadProfileImageRequest)
        {
            if (uploadProfileImageRequest == null || uploadProfileImageRequest.File == null)
            {
                _logger.LogWarning($"No image. UserId {userId}");
                return Result.Fail(PROFILE_IMAGE_NOT_FOUND).ToOldResult();
            }

            //TODO: changed how image format is validated
            string imageExtension = Path.GetExtension(uploadProfileImageRequest.File.FileName);
            if(!VALID_IMAGE_FORMATS.Contains(imageExtension.ToUpper()))
            {
                _logger.LogWarning($"Invalid image format. UserId {userId}, image extension {imageExtension}");
                return Result.Fail(INVALID_PROFILE_IMAGE_FORMAT, VALID_IMAGE_FORMATS).ToOldResult();
            }

            if(uploadProfileImageRequest.File.Length > _identityUIOptions.MaxProfileImageSize)
            {
                _logger.LogWarning($"Image is to big. UserId {userId}, image size {uploadProfileImageRequest.File.Length}");
                return Result.Fail(PROFILE_IMAGE_TOO_BIG, _identityUIOptions.MaxProfileImageSize / 1024).ToOldResult();
            }

            if (uploadProfileImageRequest.File.FileName.Length > 250)
            {
                _logger.LogWarning($"Image name is to long. Image name length {uploadProfileImageRequest.File.FileName.Length}");
                return Result.Fail(PROFILE_IMAGE_NAME_TOO_LONG, 250).ToOldResult();
            }

            byte[] image;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await uploadProfileImageRequest.File.CopyToAsync(memoryStream);

                image = memoryStream.ToArray();
            }

            string blobCacheKey = string.Format(BLOBL_CACHE_KEY, userId);
            string urlCacheKey = string.Format(URL_CACHE_KEY, userId);
            _memoryCache.Remove(blobCacheKey);
            _memoryCache.Remove(urlCacheKey);

            IBaseSpecification<UserImageEntity, UserImageEntity> getUserImageSpecification = SpecificationBuilder
                .Create<UserImageEntity>()
                .Where(x => x.UserId == userId)
                .Build();

            UserImageEntity userImage = await _userImageDAO.SingleOrDefault(getUserImageSpecification);
            if (userImage == null)
            {
                UserImageEntity newUserImage = new UserImageEntity(
                    userId: userId,
                    blobImage: image,
                    fileName: uploadProfileImageRequest.File.FileName);

                bool addResult = await _userImageDAO.Add(newUserImage);
                if (!addResult)
                {
                    _logger.LogError($"Failed to add user image. UserId {userId}");
                    return Result.Fail(FAILED_TO_ADD_PROFILE_IMAGE).ToOldResult();
                }

                return Result.Ok().ToOldResult();
            }

            userImage.BlobImage = image;
            userImage.FileName = uploadProfileImageRequest.File.FileName;

            bool updateResult = await _userImageDAO.Update(userImage);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update user image. UserId {userId}");
                return Result.Fail(FAILED_TO_UPDATE_PROFILE_IMAGE).ToOldResult();
            }

            return Result.Ok().ToOldResult();
        }

        public async Task<Core.Models.Result.Result<string>> GetProfileImageURL(string userId)
        {
            string cacheKey = string.Format(URL_CACHE_KEY, userId);

            if (_memoryCache.TryGetValue(cacheKey, out string imageUrl))
            {
                return Result.Ok<string>(imageUrl).ToOldResult();
            }

            IBaseSpecification<UserImageEntity, UserImageEntity> getUserImageSpecification = SpecificationBuilder
                .Create<UserImageEntity>()
                .Where(x => x.UserId == userId)
                .Build();

            UserImageEntity userImage = await _userImageDAO.SingleOrDefault(getUserImageSpecification);
            if (userImage == null)
            {
                _logger.LogError($"No profile image. UserId {userId}");

                return Result.Fail<string>(PROFILE_IMAGE_NOT_FOUND).ToOldResult();
            }

            _memoryCache.Set(cacheKey, userImage.URL, IMAGE_IN_CACHE_TIME_SPAN);

            return Result.Ok<string>(userImage.URL).ToOldResult();
        }

        public async Task<Core.Models.Result.Result<FileData>> GetProfileImage(string userId)
        {
            string cacheKey = string.Format(BLOBL_CACHE_KEY, userId);

            if (_memoryCache.TryGetValue(cacheKey, out FileData image))
            {
                return Result.Ok(image).ToOldResult();
            }

            IBaseSpecification<UserImageEntity, UserImageEntity> getUserImageSpecification = SpecificationBuilder
                .Create<UserImageEntity>()
                .Where(x => x.UserId == userId)
                .Build();

            UserImageEntity userImage = await _userImageDAO.SingleOrDefault(getUserImageSpecification);
            if (userImage == null)
            {
                _logger.LogInformation($"No profile image. UserId {userId}");

                Result<FileData> defaultImage = await _defaultProfileImageService.Get();
                if(defaultImage.Failure)
                {
                    return Result.Fail<FileData>(defaultImage).ToOldResult();
                }

                _memoryCache.Set(cacheKey, defaultImage.Value, IMAGE_IN_CACHE_TIME_SPAN);

                return Result.Ok(defaultImage.Value).ToOldResult();
            }

            FileData fileData = new FileData(
                fileName: userImage.FileName,
                file: userImage.BlobImage);

            _memoryCache.Set(cacheKey, fileData, IMAGE_IN_CACHE_TIME_SPAN);

            return Result.Ok(fileData).ToOldResult();
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
