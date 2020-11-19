using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Models
{
    public class ReCaptchaOptions
    {
        public bool UseReCaptcha 
        { 
            get 
            {
                if(string.IsNullOrEmpty(SiteKey) || string.IsNullOrEmpty(SiteSecret))
                {
                    return false;
                }

                return true;
            } 
        }

        public string SiteKey { get; set; }
        public string SiteSecret { get; set; }
    }
}
