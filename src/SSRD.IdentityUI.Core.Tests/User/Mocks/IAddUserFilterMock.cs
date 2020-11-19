using Moq;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IAddUserFilterMock : Mock<IAddUserFilter>
    {
        public IAddUserFilterMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IAddUserFilterMock Create()
        {
            IAddUserFilterMock addUserFilterMock = new IAddUserFilterMock();

            return addUserFilterMock;
        }

        public IAddUserFilterMock BeforeAdd_Success()
        {
            Setup(x => x.BeforeAdd(It.IsAny<BaseRegisterRequest>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }

        public IAddUserFilterMock BeforeAdd_Failure()
        {
            Setup(x => x.BeforeAdd(It.IsAny<BaseRegisterRequest>()))
                .Returns(Task.FromResult(Result.Fail(TesttConstants.RESULT_ERROR_CODE)));

            return this;
        }

        public IAddUserFilterMock AfterAdded_Success()
        {
            Setup(x => x.AfterAdded(It.IsAny<AppUserEntity>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }

        public IAddUserFilterMock AfterAdded_Failure()
        {
            Setup(x => x.AfterAdded(It.IsAny<AppUserEntity>()))
                .Returns(Task.FromResult(Result.Fail(TesttConstants.RESULT_ERROR_CODE)));

            return this;
        }
    }
}
