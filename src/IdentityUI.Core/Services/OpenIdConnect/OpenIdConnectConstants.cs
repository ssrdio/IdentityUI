using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect
{
    public static class OpenIdConnectConstants
    {
        public const string IDENTITY_UI_CLIENT_ID = "identity-ui-admin";

        public static class Scopes
        {
            public const string Permissions = "permissions";
        }

        public static readonly List<string> SCOPES = new List<string>
        {
            OpenIddictConstants.Scopes.Address,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.OfflineAccess,
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Phone,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles,
            Scopes.Permissions,
        };

        public static readonly List<string> ENDPOINTS = new List<string>
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
            OpenIddictConstants.Permissions.Endpoints.Device.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
            OpenIddictConstants.Permissions.Endpoints.Introspection.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
            OpenIddictConstants.Permissions.Endpoints.Logout.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
            OpenIddictConstants.Permissions.Endpoints.Revocation.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
            OpenIddictConstants.Permissions.Endpoints.Token.Replace(OpenIddictConstants.Permissions.Prefixes.Endpoint, ""),
        };

        public static readonly List<string> GRANT_TYPES = new List<string>
        {
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""),
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""),
            OpenIddictConstants.Permissions.GrantTypes.DeviceCode.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""),
            OpenIddictConstants.Permissions.GrantTypes.Implicit.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""),
            OpenIddictConstants.Permissions.GrantTypes.Password.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""),
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken.Replace(OpenIddictConstants.Permissions.Prefixes.GrantType, ""),
        };

        public static readonly List<string> RESPONSE_TYPES = new List<string>
        {
            OpenIddictConstants.Permissions.ResponseTypes.Code.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.CodeToken.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.IdToken.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.None.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
            OpenIddictConstants.Permissions.ResponseTypes.Token.Replace(OpenIddictConstants.Permissions.Prefixes.ResponseType, ""),
        };
    }
}
