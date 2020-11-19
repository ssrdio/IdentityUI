using Moq;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IAddInviteFilterMock : Mock<IAddInviteFilter>
    {
        public IAddInviteFilterMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IAddInviteFilterMock Create()
        {
            IAddInviteFilterMock addInviteFilterMock = new IAddInviteFilterMock();

            return addInviteFilterMock;
        }

        public IAddInviteFilterMock BeforeAdd_Success()
        {
            Setup(x => x.BeforeAdd(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }

        public IAddInviteFilterMock BeforeAdd_Failure()
        {
            Setup(x => x.BeforeAdd(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Result.Fail(TesttConstants.RESULT_ERROR_CODE)));

            return this;
        }

        public IAddInviteFilterMock AfterAdded_Success()
        {
            Setup(x => x.AfterAdded(It.IsAny<InviteEntity>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }

        public IAddInviteFilterMock AfterAdded_Failure()
        {
            Setup(x => x.AfterAdded(It.IsAny<InviteEntity>()))
                .Returns(Task.FromResult(Result.Fail(TesttConstants.RESULT_ERROR_CODE)));

            return this;
        }
    }
}
