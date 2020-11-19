using SSRD.IdentityUI.Core.Data.Models.Constants;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SSRD.IdentityUI.Core.Models.Options
{
    public class IdentityUIClaimOptions
    {
        public string UserId { get; set; } = ClaimTypes.NameIdentifier;
        public string Username { get; set; } = ClaimTypes.Name;
        public string Email { get; set; } = ClaimTypes.Email;
        public string Role { get; set; } = ClaimTypes.Role;
        public string Permission { get; set; } = IdentityUIClaims.PERMISSION;

        public string GroupId { get; set; } = IdentityUIClaims.GROUP_ID;
        public string GroupName { get; set; } = IdentityUIClaims.GROUP_NAME;
        public string GroupRole { get; set; } = IdentityUIClaims.GROUP_ROLE;
        public string GroupPermission { get; set; } = IdentityUIClaims.GROUP_PERMISSION;


        public string ImpersonatorId { get; set; } = IdentityUIClaims.IMPERSONATOR_ID;
        public string ImpersonatorUsername { get; set; } = IdentityUIClaims.IMPERSONATOR_NAME;
        public string ImpersonatorEmail { get; set; } = IdentityUIClaims.IMPERSONATOR_EMAIL;

        public string ImpersonatorRole { get; set; } = IdentityUIClaims.IMPERSONATOR_ROLE;
        public string ImpersonatorPermission { get; set; } = IdentityUIClaims.IMPERSONATOR_PERMISSION;

        public string ImpersonatorGroupId { get; set; } = IdentityUIClaims.IMPERSONATOR_GROUP_ID;
        public string ImpersonatorGroupName { get; set; } = IdentityUIClaims.IMPERSONATOR_GROUP_NAME;
        public string ImpersonatorGroupRole { get; set; } = IdentityUIClaims.IMPERSONATOR_GROUP_ROLE;
        public string ImpersonatorGroupPermission { get; set; } = IdentityUIClaims.IMPERSONATOR_GROUP_PERMISSION;

        public string UserIdentityName { get; set; } = "Identity.Application";
        public string ImpersonatorIdentityName { get; set; } = "Identity.Impersonator";
        public string SecurityStamp { get; set; } = "AspNet.Identity.SecurityStamp";
        public string SessionCode { get; set; } = "IdentityUI.SessionCode";
    }
}
