using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Server.AspNetCore;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.OpenIddict.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly IBaseDAO<AppUserEntity> _userDAO;
        private readonly IBaseDAO<RoleEntity> _roleDAO;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;
        private readonly IdentityUIEndpoints _identityUIEndpoints;

        private readonly ILogger<UserInfoController> _logger;

        public UserInfoController(
            IBaseDAO<AppUserEntity> userDAO,
            IBaseDAO<RoleEntity> roleDAO,
            IIdentityUIUserInfoService identityUIUserInfoService,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions, 
            IOptions<IdentityUIEndpoints> identityUIEndpoints,
            ILogger<UserInfoController> logger)
        {
            _userDAO = userDAO;
            _identityUIUserInfoService = identityUIUserInfoService;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
            _identityUIEndpoints = identityUIEndpoints.Value;
            _logger = logger;

        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo")]
        [Produces("application/json")]
        public async Task<IActionResult> UserInfo()
        {
            string userId = _identityUIUserInfoService.GetUserId();

            IBaseSpecification<AppUserEntity, AppUserEntity> specification = SpecificationBuilder
                .Create<AppUserEntity>()
                .Where(x => x.Id == userId)
                .Build();

            AppUserEntity user = await _userDAO.SingleOrDefault(specification);

            Dictionary<string, object> userInfo = new Dictionary<string, object>();

            userInfo.Add(_identityUIClaimOptions.UserId, user.Id);
            userInfo.Add(_identityUIClaimOptions.Username, user.UserName);

            if(!string.IsNullOrEmpty(user.Email))
            {
                userInfo.Add(_identityUIClaimOptions.Email, user.Email);
            }

            if(!string.IsNullOrEmpty(user.PhoneNumber))
            {
                userInfo.Add(_identityUIClaimOptions.PhoneNumber, user.PhoneNumber);
            }

            if(!string.IsNullOrEmpty(user.FirstName))
            {
                userInfo.Add(_identityUIClaimOptions.FirstName, user.FirstName);
            }

            if(!string.IsNullOrEmpty(user.LastName))
            {
                userInfo.Add(_identityUIClaimOptions.LastName, user.LastName);
            }

            if(!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName))
            {
                userInfo.Add(_identityUIClaimOptions.Name, $"{user.FirstName} {user.LastName}");
            }

            return Ok(userInfo);
        }
    }
}
