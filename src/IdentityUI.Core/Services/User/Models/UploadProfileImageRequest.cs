using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class UploadProfileImageRequest
    {
        public IFormFile File { get; set; }
    }
}
