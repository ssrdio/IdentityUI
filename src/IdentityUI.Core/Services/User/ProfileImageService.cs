using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    internal class ProfileImageService : IProfileImageService
    {
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

        private const string DEFAULT_PROFILE_IMAGE = "adminUI/template/vendors/fontawesome-free-5.14.0-web/svgs/solid/user-check.svg";

        private readonly IBaseRepositoryAsync<UserImageEntity> _userImageRepository;

#if NET_CORE2
        private readonly IHostingEnvironment _hostingEnvironment;
#elif NET_CORE3
        private readonly IWebHostEnvironment _hostingEnvironment;
#endif
        private readonly IMemoryCache _memoryCache;
        private readonly IdentityUIOptions _identityUIOptions;

        private readonly ILogger<ProfileImageService> _logger;

#if NET_CORE2
        public ProfileImageService(IBaseRepositoryAsync<UserImageEntity> userImageRepository, IMemoryCache memoryCache,
             IHostingEnvironment hostingEnvironment, IOptions<IdentityUIOptions> identityUIOptions, ILogger<ProfileImageService> logger)
#elif NET_CORE3
        public ProfileImageService(IBaseRepositoryAsync<UserImageEntity> userImageRepository, IMemoryCache memoryCache,
            IWebHostEnvironment hostingEnvironment, IOptions<IdentityUIOptions> identityUIOptions, ILogger<ProfileImageService> logger)
#endif
        {
            _userImageRepository = userImageRepository;
            _memoryCache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
            _identityUIOptions = identityUIOptions.Value;
            _logger = logger;
        }

        public async Task<Result> UpdateProfileImage(string userId, UploadProfileImageRequest uploadProfileImageRequest)
        {
            if (uploadProfileImageRequest == null || uploadProfileImageRequest.File == null)
            {
                _logger.LogWarning($"No image. UserId {userId}");
                return Result.Fail("no_profile_image", "No Image");
            }

            //TODO: changed how image format is validated
            string imageExtension = Path.GetExtension(uploadProfileImageRequest.File.FileName);
            if(!VALID_IMAGE_FORMATS.Contains(imageExtension.ToUpper()))
            {
                _logger.LogWarning($"Invalid image format. UserId {userId}, image extension {imageExtension}");
                return Result.Fail("profile_image_invalid_format", "Image is invalid, please select '.JPG' or '.PNG' image format.");
            }

            if(uploadProfileImageRequest.File.Length > _identityUIOptions.MaxProfileImageSize)
            {
                _logger.LogWarning($"Image is to big. UserId {userId}, image size {uploadProfileImageRequest.File.Length}");
                return Result.Fail("image_is_to_big", $"Image is to big. Max image size {_identityUIOptions.MaxProfileImageSize / 1024} kB");
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
                    fileName: uploadProfileImageRequest.File.FileName,
                    isDefault: true);

                bool addResult = await _userImageRepository.Add(newUserImage);
                if (!addResult)
                {
                    _logger.LogError($"Failed to add user image. UserId {userId}");
                    return Result.Fail("failed_to_add_user_image", "Failed to add user image");
                }

                return Result.Ok();
            }

            userImage.BlobImage = image;
            userImage.FileName = uploadProfileImageRequest.File.FileName;

            bool updateResult = await _userImageRepository.Update(userImage);
            if (!updateResult)
            {
                _logger.LogError($"Failed to update user image. UserId {userId}");
                return Result.Fail("error", "Error");
            }

            return Result.Ok();
        }

        public async Task<Result<string>> GetProfileImageURL(string userId)
        {
            string cacheKey = string.Format(URL_CACHE_KEY, userId);

            if (_memoryCache.TryGetValue(cacheKey, out string imageUrl))
            {
                return Result.Ok(imageUrl);
            }

            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            UserImageEntity userImage = await _userImageRepository.SingleOrDefault(userImageSpecification);
            if (userImage == null)
            {
                _logger.LogError($"No profile image. UserId {userId}");

                return Result.Fail<string>("no_profile_image", "No Profile image");
            }

            _memoryCache.Set(cacheKey, userImage.URL, IMAGE_IN_CACHE_TIME_SPAN);

            return Result.Ok(userImage.URL);
        }

        public async Task<Result<FileData>> GetProfileImage(string userId)
        {
            string cacheKey = string.Format(BLOBL_CACHE_KEY, userId);

            if (_memoryCache.TryGetValue(cacheKey, out FileData image))
            {
                return Result.Ok(image);
            }

            BaseSpecification<UserImageEntity> userImageSpecification = new BaseSpecification<UserImageEntity>();
            userImageSpecification.AddFilter(x => x.UserId == userId);

            UserImageEntity userImage = await _userImageRepository.SingleOrDefault(userImageSpecification);
            if (userImage == null)
            {
                _logger.LogError($"No profile image. UserId {userId}");

                return GetDefaultImage();
            }

            FileData fileData = new FileData(
                fileName: userImage.FileName,
                file: userImage.BlobImage);

            _memoryCache.Set(cacheKey, fileData, IMAGE_IN_CACHE_TIME_SPAN);

            return Result.Ok(fileData);
        }

        private Result<FileData> GetDefaultImage()
        {
            return Result.Fail<FileData>("error", "Error");
            //TODO: return default image
        }
    }
}
