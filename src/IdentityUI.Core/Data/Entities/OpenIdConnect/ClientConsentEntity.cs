using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect
{
    public class ClientConsentEntity : OpenIddictEntityFrameworkCoreAuthorization<string, ClientEntity, ClientTokenEntity>, IIdentityUIEntity
    {
    }
}
