using Microsoft.AspNetCore.Identity;
using Moq;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class SignInManagerMock : Mock<SignInManager<AppUserEntity>>
    {
        public SignInManagerMock(MockBehavior mockBehavior = MockBehavior.Strict) 
            : base(mockBehavior, UserManagerMock.Create().Object, IHttpContextAccessorMock.Create().Object, new Mock<IUserClaimsPrincipalFactory<AppUserEntity>>().Object, null, null, null, null)
        {
        }

        public static SignInManagerMock Create()
        {
            SignInManagerMock signInManagerMock = new SignInManagerMock(MockBehavior.Default);

            return signInManagerMock;
        }
    }
}
