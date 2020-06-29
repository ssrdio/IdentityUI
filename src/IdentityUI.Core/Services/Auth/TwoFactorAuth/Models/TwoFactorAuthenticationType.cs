using Microsoft.AspNetCore.Identity;

namespace SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models
{
    public enum TwoFactorAuthenticationType
    {
        None = 0,
        Authenticator = 1,
        Email = 2,
        Phone = 3,
    }

    public static class TwoFactorAuthenticationTypeExtensions
    {
        public static string ToProvider(this TwoFactorAuthenticationType value)
        {
            switch (value)
            {
                case TwoFactorAuthenticationType.Authenticator:
                    return TokenOptions.DefaultAuthenticatorProvider;
                case TwoFactorAuthenticationType.Email:
                    return TokenOptions.DefaultEmailProvider;
                case TwoFactorAuthenticationType.Phone:
                    return TokenOptions.DefaultPhoneProvider;
                default:
                    return string.Empty;
            }
        }
    }
}
