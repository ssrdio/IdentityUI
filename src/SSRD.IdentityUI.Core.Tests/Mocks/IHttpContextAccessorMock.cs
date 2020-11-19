using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IHttpContextAccessorMock : Mock<IHttpContextAccessor>
    {
        public IHttpContextAccessorMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IHttpContextAccessorMock Create()
        {
            IHttpContextAccessorMock httpContextAccessorMock = new IHttpContextAccessorMock();

            return httpContextAccessorMock;
        }
    }
}
