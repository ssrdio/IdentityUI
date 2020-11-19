using Moq;
using SSRD.IdentityUI.Core.Interfaces;

namespace SSRD.IdentityUI.Core.Tests.Group.Mocks
{
    public class IGroupUserStoreMock : Mock<IGroupUserStore>
    {
        public IGroupUserStoreMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IGroupUserStoreMock Create()
        {
            IGroupUserStoreMock groupUserStoreMock = new IGroupUserStoreMock();

            return groupUserStoreMock;
        }
    }
}
