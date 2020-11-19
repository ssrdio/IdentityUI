using Moq;
using SSRD.IdentityUI.Core.Interfaces;

namespace SSRD.IdentityUI.Core.Tests.Group.Mocks
{
    public class IGroupStoreMock : Mock<IGroupStore>
    {
        public IGroupStoreMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IGroupStoreMock Create()
        {
            IGroupStoreMock groupStoreMock = new IGroupStoreMock();

            return groupStoreMock;
        }
    }
}
