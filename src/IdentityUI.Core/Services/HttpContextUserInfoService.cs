using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Security.Claims;

namespace SSRD.IdentityUI.Core.Services
{
    public class HttpContextUserInfoService : IIdentityUIUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        public HttpContextUserInfoService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
        }

        public string GetGroupId()
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/IdentityAdmin"))
            {
                return null;
            }

            return _httpContextAccessor.HttpContext.User.GetGroupId(_identityUIClaimOptions);
        }

        public string GetImpersonatorId()
        {
            return _httpContextAccessor.HttpContext.User.GetImpersonatorId(_identityUIClaimOptions);
        }

        public string GetSessionCode()
        {
            return _httpContextAccessor.HttpContext.User.GetSessionCode(_identityUIClaimOptions);
        }

        public string GetUserId()
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            if(_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/IdentityAdmin"))
            {
                if(_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.GetImpersonatorId(_identityUIClaimOptions);
                }
            }

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/GroupAdmin")
                || _httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/api/GroupAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.GetImpersonatorId(_identityUIClaimOptions);
                }
            }

            return _httpContextAccessor.HttpContext.User.GetUserId(_identityUIClaimOptions);
        }

        public string GetUsername()
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/IdentityAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.GetImpersonatorUsername(_identityUIClaimOptions);
                }
            }

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/GroupAdmin")
                || _httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/api/GroupAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.GetImpersonatorUsername(_identityUIClaimOptions);
                }
            }

            return user.GetUsername(_identityUIClaimOptions);
        }

        public bool HasGroupPermission(string permission)
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/IdentityAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.ImpersonatorHasGroupPermission(permission, _identityUIClaimOptions);
                }
            }

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/GroupAdmin")
                || _httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/api/GroupAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.ImpersonatorHasGroupPermission(permission, _identityUIClaimOptions);
                }
            }

            return _httpContextAccessor.HttpContext.User.HasGroupPermission(permission, _identityUIClaimOptions);
        }

        public bool HasPermission(string permission)
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/IdentityAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.ImpersonatorHasPermission(permission, _identityUIClaimOptions);
                }
            }

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/GroupAdmin")
                || _httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/api/GroupAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.ImpersonatorHasPermission(permission, _identityUIClaimOptions);
                }
            }

            return _httpContextAccessor.HttpContext.User.HasPermission(permission, _identityUIClaimOptions);
        }

        public bool HasRole(string role)
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/IdentityAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.ImpersonatorHasRole(role, _identityUIClaimOptions);
                }
            }

            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/GroupAdmin")
                || _httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/api/GroupAdmin"))
            {
                if (_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
                {
                    return user.ImpersonatorHasRole(role, _identityUIClaimOptions);
                }
            }

            return _httpContextAccessor.HttpContext.User.HasRole(role, _identityUIClaimOptions);
        }
    }
}
