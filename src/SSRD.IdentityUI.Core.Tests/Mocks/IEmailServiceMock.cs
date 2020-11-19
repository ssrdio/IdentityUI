using Moq;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IEmailServiceMock : Mock<IEmailService>
    {
        public IEmailServiceMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IEmailServiceMock Create()
        {
            IEmailServiceMock emailServiceMock = new IEmailServiceMock();

            return emailServiceMock;
        }

        public IEmailServiceMock SendInvite_Success()
        {
            Setup(x => x.SendInvite(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(Result.Ok()));

            return this;
        }
    }
}
