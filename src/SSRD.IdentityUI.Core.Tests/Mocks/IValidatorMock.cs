using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IValidatorMock<TModel> : Mock<IValidator<TModel>>
    {
        public IValidatorMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IValidatorMock<TModel> Create()
        {
            IValidatorMock<TModel> validatorMock = new IValidatorMock<TModel>();

            return validatorMock;
        }

        public static IValidatorMock<TModel> CreateLoose()
        {
            IValidatorMock<TModel> validatorMock = new IValidatorMock<TModel>(MockBehavior.Loose);

            return validatorMock;
        }

        public IValidatorMock<TModel> Validate_Valid()
        {
            Setup(x => x.Validate(It.IsAny<TModel>()))
                .Returns(new ValidationResult(new List<ValidationFailure>()));

            Setup(x => x.Validate(It.IsAny<ValidationContext>()))
                .Returns(new ValidationResult(new List<ValidationFailure>()));

            return this;
        }
    }
}
