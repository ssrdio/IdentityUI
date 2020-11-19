using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Services
{
    public interface IReCaptchaRequest
    {
        string ReCaptchaToken { get; set; }
    }
}
