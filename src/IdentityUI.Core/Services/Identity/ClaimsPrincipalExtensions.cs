using Microsoft.EntityFrameworkCore.Internal;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Identity
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetGroupId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == IdentityUIClaims.GROUP_ID)
                .Select(x => x.Value)
                .SingleOrDefault();
        }

        public static bool HasRole(this ClaimsPrincipal claimsPrincipal, string role)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Where(x => x.Value == role)
                .Any();
        }

        public static bool HasGroupRole(this ClaimsPrincipal claimsPrincipal, string role)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == ClaimTypes.Role || x.Type == IdentityUIClaims.GROUP_ROLE)
                .Where(x => x.Value == role)
                .Any();
        }

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value);
        }

        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permission)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == IdentityUIClaims.PERMISSION)
                .Where(x => x.Value == permission)
                .Any();
        }

        public static bool HasGroupPermission(this ClaimsPrincipal claimsPrincipal, string permission)
        {
            return claimsPrincipal.Claims
                .Where(x => x.Type == IdentityUIClaims.PERMISSION || x.Type == IdentityUIClaims.GROUP_PERMISSION)
                .Where(x => x.Value == permission)
                .Any();
        }

        public static string GetSessionCode(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(IdentityUIClaims.SESSION_CODE);
        }
    }
}
