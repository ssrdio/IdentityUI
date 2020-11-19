using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.User;
using SSRD.IdentityUI.Core.Services.User.Models;
using SSRD.IdentityUI.Core.Tests.Group.Mocks;
using SSRD.IdentityUI.Core.Tests.Mocks;
using System.Threading.Tasks;
using Xunit;

namespace SSRD.IdentityUI.Core.Tests.User
{
    public class AddUserServiceTestes
    {
        [Fact]
        public async Task AddUser_BeforeAdd_Success()
        {
            AddUserService addUserService = new AddUserService(
                UserManagerMock.Create().WithCreateSuccess().Object,
                SignInManagerMock.Create().Object,
                IEmailConfirmationServiceMock.Create().SendVerificationMailSuccess().Object,
                IGroupUserServiceMock.Create().Object,
                IAddUserFilterMock.Create().BeforeAdd_Success().AfterAdded_Success().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object,
                IValidatorMock<NewUserRequest>.Create().Object,
                IValidatorMock<RegisterRequest>.Create().Object,
                IValidatorMock<AcceptInviteRequest>.Create().Object,
                IValidatorMock<ExternalLoginRegisterRequest>.Create().Object,
                IValidatorMock<GroupBaseUserRegisterRequest>.Create().Object,
                IValidatorMock<BaseRegisterRequest>.Create().Validate_Valid().Object,
                IValidatorMock<IUserAttributeRequest>.Create().Validate_Valid().Object,
                NullLogger<AddUserService>.Instance);

            BaseRegisterRequest baseRegisterRequest = new Fixture()
                .Build<BaseRegisterRequest>()
                .Create();

            Result<AppUserEntity> result = await addUserService.AddUser(baseRegisterRequest);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_BeforeAdd_Failure()
        {
            AddUserService addUserService = new AddUserService(
                UserManagerMock.Create().WithCreateSuccess().Object,
                SignInManagerMock.Create().Object,
                IEmailConfirmationServiceMock.Create().SendVerificationMailSuccess().Object,
                IGroupUserServiceMock.Create().Object,
                IAddUserFilterMock.Create().BeforeAdd_Failure().AfterAdded_Success().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object,
                IValidatorMock<NewUserRequest>.Create().Object,
                IValidatorMock<RegisterRequest>.Create().Object,
                IValidatorMock<AcceptInviteRequest>.Create().Object,
                IValidatorMock<ExternalLoginRegisterRequest>.Create().Object,
                IValidatorMock<GroupBaseUserRegisterRequest>.Create().Object,
                IValidatorMock<BaseRegisterRequest>.Create().Validate_Valid().Object,
                IValidatorMock<IUserAttributeRequest>.Create().Validate_Valid().Object,
                NullLogger<AddUserService>.Instance);

            BaseRegisterRequest baseRegisterRequest = new Fixture()
                .Build<BaseRegisterRequest>()
                .Create();

            Result<AppUserEntity> result = await addUserService.AddUser(baseRegisterRequest);

            result.Success.Should().BeFalse();
            result.ResultMessages.Should().Contain(x => x.Code == TesttConstants.RESULT_ERROR_CODE);
        }

        [Fact]
        public async Task AddUser_AfterAdded_Success()
        {
            AddUserService addUserService = new AddUserService(
                UserManagerMock.Create().WithCreateSuccess().Object,
                SignInManagerMock.Create().Object,
                IEmailConfirmationServiceMock.Create().SendVerificationMailSuccess().Object,
                IGroupUserServiceMock.Create().Object,
                IAddUserFilterMock.Create().BeforeAdd_Success().AfterAdded_Success().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object,
                IValidatorMock<NewUserRequest>.Create().Object,
                IValidatorMock<RegisterRequest>.Create().Object,
                IValidatorMock<AcceptInviteRequest>.Create().Object,
                IValidatorMock<ExternalLoginRegisterRequest>.Create().Object,
                IValidatorMock<GroupBaseUserRegisterRequest>.Create().Object,
                IValidatorMock<BaseRegisterRequest>.Create().Validate_Valid().Object,
                IValidatorMock<IUserAttributeRequest>.Create().Validate_Valid().Object,
                NullLogger<AddUserService>.Instance);

            BaseRegisterRequest baseRegisterRequest = new Fixture()
                .Build<BaseRegisterRequest>()
                .Create();

            Result<AppUserEntity> result = await addUserService.AddUser(baseRegisterRequest);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_AfterAdded_Failure()
        {
            AddUserService addUserService = new AddUserService(
                UserManagerMock.Create().WithCreateSuccess().Object,
                SignInManagerMock.Create().Object,
                IEmailConfirmationServiceMock.Create().SendVerificationMailSuccess().Object,
                IGroupUserServiceMock.Create().Object,
                IAddUserFilterMock.Create().BeforeAdd_Success().AfterAdded_Failure().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object,
                IValidatorMock<NewUserRequest>.Create().Object,
                IValidatorMock<RegisterRequest>.Create().Object,
                IValidatorMock<AcceptInviteRequest>.Create().Object,
                IValidatorMock<ExternalLoginRegisterRequest>.Create().Object,
                IValidatorMock<GroupBaseUserRegisterRequest>.Create().Object,
                IValidatorMock<BaseRegisterRequest>.Create().Validate_Valid().Object,
                IValidatorMock<IUserAttributeRequest>.Create().Validate_Valid().Object,
                NullLogger<AddUserService>.Instance);

            BaseRegisterRequest baseRegisterRequest = new Fixture()
                .Build<BaseRegisterRequest>()
                .Create();

            Result<AppUserEntity> result = await addUserService.AddUser(baseRegisterRequest);

            result.Success.Should().BeFalse();
            result.ResultMessages.Should().Contain(x => x.Code == TesttConstants.RESULT_ERROR_CODE);
        }
    }
}
