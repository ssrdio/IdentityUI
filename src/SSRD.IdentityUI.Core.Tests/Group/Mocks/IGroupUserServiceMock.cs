using Moq;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;

namespace SSRD.IdentityUI.Core.Tests.Group.Mocks
{
    public class IGroupUserServiceMock : Mock<IGroupUserService>
    {
        public IGroupUserServiceMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IGroupUserServiceMock Create()
        {
            IGroupUserServiceMock groupUserServiceMock = new IGroupUserServiceMock();

            return groupUserServiceMock;
        }
    }
}
