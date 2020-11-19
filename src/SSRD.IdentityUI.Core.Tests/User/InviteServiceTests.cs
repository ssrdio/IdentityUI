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
    public class InviteServiceTests
    {
        [Fact]
        public async Task Add_BeforeAdd_Success()
        {
            InviteService inviteService = new InviteService(
                IBaseDAOMock<AppUserEntity>.Create().Exists_Failure().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Add_Success().Exists_Failure().Object,
                IGroupStoreMock.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IEmailServiceMock.Create().SendInvite_Success().Object,
                IAddInviteFilterMock.Create().BeforeAdd_Success().AfterAdded_Success().Object,
                IValidatorMock<InviteToGroupRequest>.Create().Object,
                IValidatorMock<InviteRequest>.Create().Object,
                NullLogger<InviteService>.Instance,
                IOptionsMock<IdentityUIOptions>.Create().DefaultValue().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object);

            Result result = await inviteService.AddInvite("email");

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Add_BeforeAdd_Failure()
        {
            InviteService inviteService = new InviteService(
                IBaseDAOMock<AppUserEntity>.Create().Exists_Failure().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Add_Success().Exists_Failure().Object,
                IGroupStoreMock.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IEmailServiceMock.Create().SendInvite_Success().Object,
                IAddInviteFilterMock.Create().BeforeAdd_Failure().AfterAdded_Success().Object,
                IValidatorMock<InviteToGroupRequest>.Create().Object,
                IValidatorMock<InviteRequest>.Create().Object,
                NullLogger<InviteService>.Instance,
                IOptionsMock<IdentityUIOptions>.Create().DefaultValue().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object);

            Result result = await inviteService.AddInvite("email");

            result.Success.Should().BeFalse();
            result.ResultMessages.Should().Contain(x => x.Code == TesttConstants.RESULT_ERROR_CODE);
        }

        [Fact]
        public async Task Add_AfterAdded_Success()
        {
            InviteService inviteService = new InviteService(
                IBaseDAOMock<AppUserEntity>.Create().Exists_Failure().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Add_Success().Exists_Failure().Object,
                IGroupStoreMock.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IEmailServiceMock.Create().SendInvite_Success().Object,
                IAddInviteFilterMock.Create().BeforeAdd_Success().AfterAdded_Success().Object,
                IValidatorMock<InviteToGroupRequest>.Create().Object,
                IValidatorMock<InviteRequest>.Create().Object,
                NullLogger<InviteService>.Instance,
                IOptionsMock<IdentityUIOptions>.Create().DefaultValue().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object);

            Result result = await inviteService.AddInvite("email");

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Add_AfterAdded_Failure()
        {
            InviteService inviteService = new InviteService(
                IBaseDAOMock<AppUserEntity>.Create().Exists_Failure().Object,
                IBaseDAOMock<RoleEntity>.Create().Object,
                IBaseDAOMock<InviteEntity>.Create().Add_Success().Exists_Failure().Object,
                IGroupStoreMock.Create().Object,
                IGroupUserStoreMock.Create().Object,
                IEmailServiceMock.Create().SendInvite_Success().Object,
                IAddInviteFilterMock.Create().BeforeAdd_Success().AfterAdded_Failure().Object,
                IValidatorMock<InviteToGroupRequest>.Create().Object,
                IValidatorMock<InviteRequest>.Create().Object,
                NullLogger<InviteService>.Instance,
                IOptionsMock<IdentityUIOptions>.Create().DefaultValue().Object,
                IOptionsMock<IdentityUIEndpoints>.Create().DefaultValue().Object);

            Result result = await inviteService.AddInvite("email");

            result.Success.Should().BeFalse();
            result.ResultMessages.Should().Contain(x => x.Code == TesttConstants.RESULT_ERROR_CODE);
        }
    }
}
