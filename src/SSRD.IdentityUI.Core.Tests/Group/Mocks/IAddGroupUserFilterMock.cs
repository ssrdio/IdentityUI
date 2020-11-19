using Moq;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Group.Mocks
{
    public class IAddGroupUserFilterMock : Mock<IAddGroupUserFilter>
    {
        public IAddGroupUserFilterMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IAddGroupUserFilterMock Create()
        {
            IAddGroupUserFilterMock addGroupUserMock = new IAddGroupUserFilterMock();

            return addGroupUserMock;
        }

        public IAddGroupUserFilterMock BeforeAdd_Success()
        {
            Setup(x => x.BeforeAdd(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }

        public IAddGroupUserFilterMock BeforeAdd_Failure()
        {
            Setup(x => x.BeforeAdd(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Result.Fail(TesttConstants.RESULT_ERROR_CODE)));

            return this;
        }

        public IAddGroupUserFilterMock AfterAdded_Success()
        {
            Setup(x => x.AfterAdded(It.IsAny<GroupUserEntity>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }

        public IAddGroupUserFilterMock AfterAdded_Failure()
        {
            Setup(x => x.AfterAdded(It.IsAny<GroupUserEntity>()))
                .Returns(Task.FromResult(Result.Fail(TesttConstants.RESULT_ERROR_CODE)));

            return this;
        }
    }
}
