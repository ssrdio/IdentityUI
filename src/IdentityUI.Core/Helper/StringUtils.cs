using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Helper
{
    public static class StringUtils
    {
        public static string GenerateToken(int lenght = 40)
        {
            Random random = new Random();
            string token = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", lenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return token;
        }
    }
}
