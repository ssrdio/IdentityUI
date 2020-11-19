using Moq;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IEmailConfirmationServiceMock : Mock<IEmailConfirmationService>
    {
        public IEmailConfirmationServiceMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IEmailConfirmationServiceMock Create()
        {
            IEmailConfirmationServiceMock emailConfirmationServiceMock = new IEmailConfirmationServiceMock();

            return emailConfirmationServiceMock;
        }

        public IEmailConfirmationServiceMock SendVerificationMailSuccess()
        {
            Setup(x => x.SendVerificationMail(It.IsAny<AppUserEntity>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }
    }
}
