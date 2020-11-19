using AutoFixture;
using AutoFixture.Dsl;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Entities.User;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Core.Tests.Fixtures.User
{
    public class AppUserFixture
    {
        public static IPostprocessComposer<AppUserEntity> Build()
        {
            Fixture fixture = new Fixture();

            IPostprocessComposer<AppUserEntity> composer = fixture
                .Build<AppUserEntity>()
                .With(x => x.Claims, new List<UserClaimEntity>())
                .With(x => x.Logins, new List<UserLoginEntity>())
                .With(x => x.Tokens, new List<UserTokenEntity>())
                .With(x => x.UserRoles, new List<UserRoleEntity>())
                .With(x => x.Sessions, new List<SessionEntity>())
                .With(x => x.Groups, new List<GroupUserEntity>())
                .With(x => x.UserImage, new UserImageEntity())
                .With(x => x.Attributes, new List<UserAttributeEntity>());

            return composer;
        }
    }
}
