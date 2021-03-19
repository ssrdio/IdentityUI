using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.OpenIddict
{
    public static class OpendictEntityConfigurationExtensions
    {
        public static void ConfigureOpendict(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new OpenIddictEntityFrameworkCoreApplicationConfiguration<
                ClientEntity,
                ClientConsentEntity,
                ClientTokenEntity,
                string>());

            builder.Entity<ClientEntity>()
                .ToTable("Clients");

            builder.ApplyConfiguration(new OpenIddictEntityFrameworkCoreAuthorizationConfiguration<
                ClientConsentEntity,
                ClientEntity,
                ClientTokenEntity,
                string>());

            builder.Entity<ClientConsentEntity>()
                .ToTable("ClientConsents");

            builder.ApplyConfiguration(new OpenIddictEntityFrameworkCoreScopeConfiguration<ClientScopeEntity, string>());

            builder.Entity<ClientScopeEntity>()
                .ToTable("ClientScopes");

            builder.ApplyConfiguration(new OpenIddictEntityFrameworkCoreTokenConfiguration<
                ClientTokenEntity,
                ClientEntity,
                ClientConsentEntity,
                string>());

            builder.Entity<ClientTokenEntity>()
                .ToTable("ClientTokens");
        }
    }
}
