using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public static class ClaimsPrincipalExtensions
    {
        [Obsolete("Use GetUserId(this HttpContext context) or GetUserId(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [Obsolete("Use GetUsername(this HttpContext context) or GetUsername(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        }

        [Obsolete("Use GetGroupId(this HttpContext context) or GetGroupId(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static string GetGroupId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == IdentityUIClaims.GROUP_ID)
                .Select(x => x.Value)
                .SingleOrDefault();
        }

        [Obsolete("Use HasRole(this HttpContext context, string role) or HasRole(this ClaimsPrincipal claimsPrincipal, string role, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static bool HasRole(this ClaimsPrincipal claimsPrincipal, string role)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Where(x => x.Value == role)
                .Any();
        }

        [Obsolete("Use HasGroupRole(this HttpContext context, string role) or HasGroupRole(this ClaimsPrincipal claimsPrincipal, string role, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static bool HasGroupRole(this ClaimsPrincipal claimsPrincipal, string role)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == ClaimTypes.Role || x.Type == IdentityUIClaims.GROUP_ROLE)
                .Where(x => x.Value == role)
                .Any();
        }

        [Obsolete("Use GetRoles(this HttpContext context) or GetRoles(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value);
        }

        [Obsolete("Use HasPermission(this HttpContext context, string permission) or HasPermission(this ClaimsPrincipal claimsPrincipal, string permission, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permission)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == IdentityUIClaims.PERMISSION)
                .Where(x => x.Value == permission)
                .Any();
        }

        [Obsolete("Use HasGroupPermission(this HttpContext context, string permission) or HasGroupPermission(this ClaimsPrincipal claimsPrincipal, string permission, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static bool HasGroupPermission(this ClaimsPrincipal claimsPrincipal, string permission)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == IdentityUIClaims.PERMISSION || x.Type == IdentityUIClaims.GROUP_PERMISSION)
                .Where(x => x.Value == permission)
                .Any();
        }

        [Obsolete("Use GetSessionCode(this HttpContext context) or GetSessionCode(this ClaimsPrincipal principal, IdentityUIClaimOptions identityUIClaimOptions)")]
        public static string GetSessionCode(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(IdentityUIClaims.SESSION_CODE);
        }

        public static string GetUserId(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetUserId(identityUIClaimOptions);
        }

        public static string GetUserId(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.FindFirstValue(identityUIClaimOptions.UserId);
        }

        public static string GetImpersonatorId(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetImpersonatorId(identityUIClaimOptions);
        }

        public static string GetImpersonatorId(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.FindFirstValue(identityUIClaimOptions.ImpersonatorId);
        }

        public static string GetImpersonatorUsername(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetImpersonatorUsername(identityUIClaimOptions);
        }

        public static string GetImpersonatorUsername(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.FindFirstValue(identityUIClaimOptions.ImpersonatorUsername);
        }

        public static bool HasRole(this HttpContext context, string role)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.HasRole(role, identityUIClaimOptions);
        }

        public static bool HasRole(this ClaimsPrincipal claimsPrincipal, string role, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.Role)
                .Where(x => x.Value == role)
                .Any();
        }

        public static bool ImpersonatorHasRole(this HttpContext context, string role)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.ImpersonatorHasRole(role, identityUIClaimOptions);
        }

        public static bool ImpersonatorHasRole(this ClaimsPrincipal claimsPrincipal, string role, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.ImpersonatorRole)
                .Where(x => x.Value == role)
                .Any();
        }

        public static bool ImpersonatorHasGroupPermission(this HttpContext context, string permission)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.ImpersonatorHasGroupPermission(permission, identityUIClaimOptions);
        }

        public static bool ImpersonatorHasGroupPermission(this ClaimsPrincipal claimsPrincipal, string permission, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.ImpersonatorGroupPermission || x.Type == identityUIClaimOptions.ImpersonatorPermission)
                .Where(x => x.Value == permission)
                .Any();
        }

        public static bool ImpersonatorHasPermission(this HttpContext context, string permission)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.ImpersonatorHasPermission(permission, identityUIClaimOptions);
        }

        public static bool ImpersonatorHasPermission(this ClaimsPrincipal claimsPrincipal, string permission, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.ImpersonatorPermission)
                .Where(x => x.Value == permission)
                .Any();
        }

        public static bool IsImpersonized(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.IsImpersonized(identityUIClaimOptions);
        }

        public static bool IsImpersonized(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.HasClaim(x => x.Type == identityUIClaimOptions.ImpersonatorId);
        }

        public static string GetSessionCode(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetSessionCode(identityUIClaimOptions);
        }

        public static string GetSessionCode(this ClaimsPrincipal principal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return principal.FindFirstValue(identityUIClaimOptions.SessionCode);
        }

        public static string GetUsername(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetUsername(identityUIClaimOptions);
        }

        public static string GetUsername(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.FindFirstValue(identityUIClaimOptions.Username);
        }

        public static string GetGroupId(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetGroupId(identityUIClaimOptions);
        }

        public static string GetGroupId(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.GroupId)
                .Select(x => x.Value)
                .SingleOrDefault();
        }

        public static bool HasGroupRole(this HttpContext context, string role)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.HasGroupRole(role, identityUIClaimOptions);
        }

        public static bool HasGroupRole(this ClaimsPrincipal claimsPrincipal, string role, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.Role || x.Type == identityUIClaimOptions.GroupRole)
                .Where(x => x.Value == role)
                .Any();
        }

        public static IEnumerable<string> GetRoles(this HttpContext context)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.GetRoles(identityUIClaimOptions);
        }

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal claimsPrincipal, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.Role)
                .Select(x => x.Value);
        }

        public static bool HasPermission(this HttpContext context, string permission)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.HasPermission(permission, identityUIClaimOptions);
        }

        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permission, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.Permission)
                .Where(x => x.Value == permission)
                .Any();
        }

        public static bool HasGroupPermission(this HttpContext context, string permission)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.HasGroupPermission(permission, identityUIClaimOptions);
        }

        public static bool HasGroupPermission(this ClaimsPrincipal claimsPrincipal, string permission, IdentityUIClaimOptions identityUIClaimOptions)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == identityUIClaimOptions.Permission || x.Type == identityUIClaimOptions.GroupPermission)
                .Where(x => x.Value == permission)
                .Any();
        }

        public static bool HasGroupPermissionOrImpersonatorHasPermission(this HttpContext context, string permission)
        {
            IdentityUIClaimOptions identityUIClaimOptions = context.RequestServices.GetRequiredService<IOptions<IdentityUIClaimOptions>>().Value;

            return context.User.HasGroupPermissionOrImpersonatorHasPermission(permission, identityUIClaimOptions);
        }

        public static bool HasGroupPermissionOrImpersonatorHasPermission(
            this ClaimsPrincipal claimsPrincipal,
            string permission,
            IdentityUIClaimOptions identityUIClaimOptions)
        {
            bool hasPermission = claimsPrincipal.HasGroupPermission(permission, identityUIClaimOptions);
            if(hasPermission)
            {
                return true;
            }

            bool ImpersonatorHasPermission = claimsPrincipal.ImpersonatorHasGroupPermission(permission, identityUIClaimOptions);
            if(ImpersonatorHasPermission)
            {
                return true;
            }

            return false;
        }
    }
}
