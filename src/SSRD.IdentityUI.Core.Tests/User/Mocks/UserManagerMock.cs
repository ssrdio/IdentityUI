using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class UserManagerMock : Mock<UserManager<AppUserEntity>>
    {
        public UserManagerMock(MockBehavior mockBehavior = MockBehavior.Strict)
            : base(mockBehavior, new Mock<IUserStore<AppUserEntity>>(MockBehavior.Strict).Object, null, null, null, null, null, null, null, NullLogger<UserManager<AppUserEntity>>.Instance)
        {
        }

        public static UserManagerMock Create()
        {
            UserManagerMock userStoreMock = new UserManagerMock(MockBehavior.Default);

            return userStoreMock;
        }

        public UserManagerMock WithCreateSuccess()
        {
            Setup(x => x.CreateAsync(It.IsAny<AppUserEntity>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            return this;
        }
    }
}
