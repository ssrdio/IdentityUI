using SSRD.AdminUI.Template.Models;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage
{
    public class TwoFactorAuthenticatorViewModel
    {
        public bool TwoFactorAuthenticationEnabled { get; set; }
        public string TwoFactorAuthenticationName { get; set; }


        public bool IsPhoneAuthenticationAvailable { get; set; }

        public bool IsEmailAuthenticationAvailable { get; set; }


        public StatusAlertViewModel StatusAlert { get; set; }

        public TwoFactorAuthenticatorViewModel(bool twoFactorAuthenticationEnabled, string twoFactorAuthenticationName,
            bool isPhoneAuthenticationAvailable, bool isEmailAuthenticationAvailable)
        {
            TwoFactorAuthenticationEnabled = twoFactorAuthenticationEnabled;
            TwoFactorAuthenticationName = twoFactorAuthenticationName;

            IsPhoneAuthenticationAvailable = isPhoneAuthenticationAvailable;
            IsEmailAuthenticationAvailable = isEmailAuthenticationAvailable;
        }

        public TwoFactorAuthenticatorViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
