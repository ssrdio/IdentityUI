using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IIdentityUIUserInfoService
    {
        string GetUserId();
        string GetGroupId();
        string GetSessionCode();

        string GetImpersonatorId();

        bool HasPermission(string permission);
        bool HasGroupPermission(string permission);

        bool HasRole(string role);

        string GetUsername();
    }
}
