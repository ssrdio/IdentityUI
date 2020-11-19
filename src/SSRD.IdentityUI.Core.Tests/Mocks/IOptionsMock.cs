using Microsoft.Extensions.Options;
using Moq;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IOptionsMock<TModel> : Mock<IOptions<TModel>>
        where TModel : class, new()
    {
        public IOptionsMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IOptionsMock<TModel> Create()
        {
            IOptionsMock<TModel> optionsMock = new IOptionsMock<TModel>();

            return optionsMock;
        }

        public IOptionsMock<TModel> DefaultValue()
        {
            Setup(x => x.Value)
                .Returns(new TModel());

            return this;
        }

        public IOptionsMock<TModel> WithValue(TModel model)
        {
            Setup(x => x.Value)
                .Returns(model);

            return this;
        }
    }
}
