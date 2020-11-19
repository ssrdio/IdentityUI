using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Services.Group;
using SSRD.IdentityUI.Core.Services.Group.Models;
using SSRD.IdentityUI.Core.Tests.Group.Mocks;
using SSRD.IdentityUI.Core.Tests.Mocks;
using System.Threading.Tasks;
using Xunit;

namespace SSRD.IdentityUI.Core.Tests.Group
{
    public class AddGroupUserTests
    {
        [Fact]
        public async Task AddUserToGroupWithoutValidation_BeforeAdd_Success()
        {
            GroupUserService groupUserService = new GroupUserService(
                IBaseDAOMock<GroupUserEntity>.Create().Add_Success().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<GroupEntity>.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IAddGroupUserFilterMock.Create().BeforeAdd_Success().AfterAdded_Success().Object,
                IValidatorMock<AddExistingUserRequest>.Create().Object,
                NullLogger<GroupUserService>.Instance);

            Result<GroupUserEntity> result = await groupUserService.AddUserToGroupWithoutValidation("user_id", "group_id", "role_id");

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task AddUserToGroupWithoutValidation_BeforeAdd_Failure()
        {
            GroupUserService groupUserService = new GroupUserService(
                IBaseDAOMock<GroupUserEntity>.Create().Add_Success().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<GroupEntity>.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IAddGroupUserFilterMock.Create().BeforeAdd_Failure().AfterAdded_Success().Object,
                IValidatorMock<AddExistingUserRequest>.Create().Object,
                NullLogger<GroupUserService>.Instance);

            Result<GroupUserEntity> result = await groupUserService.AddUserToGroupWithoutValidation("user_id", "group_id", "role_id");

            result.Success.Should().BeFalse();
            result.ResultMessages.Should().Contain(x => x.Code == TesttConstants.RESULT_ERROR_CODE);
        }

        [Fact]
        public async Task AddUserToGroupWithoutValidation_AfterAdded_Success()
        {
            GroupUserService groupUserService = new GroupUserService(
                IBaseDAOMock<GroupUserEntity>.Create().Add_Success().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<GroupEntity>.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IAddGroupUserFilterMock.Create().BeforeAdd_Success().AfterAdded_Success().Object,
                IValidatorMock<AddExistingUserRequest>.Create().Object,
                NullLogger<GroupUserService>.Instance);

            Result<GroupUserEntity> result = await groupUserService.AddUserToGroupWithoutValidation("user_id", "group_id", "role_id");

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task AddUserToGroupWithoutValidation_AfterAdded_Failure()
        {
            GroupUserService groupUserService = new GroupUserService(
                IBaseDAOMock<GroupUserEntity>.Create().Add_Success().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<AppUserEntity>.Create().Object,
                IBaseDAOMock<GroupEntity>.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IAddGroupUserFilterMock.Create().BeforeAdd_Success().AfterAdded_Failure().Object,
                IValidatorMock<AddExistingUserRequest>.Create().Object,
                NullLogger<GroupUserService>.Instance);

            Result<GroupUserEntity> result = await groupUserService.AddUserToGroupWithoutValidation("user_id", "group_id", "role_id");

            result.Success.Should().BeFalse();
            result.ResultMessages.Should().Contain(x => x.Code == TesttConstants.RESULT_ERROR_CODE);
        }
    }
}
