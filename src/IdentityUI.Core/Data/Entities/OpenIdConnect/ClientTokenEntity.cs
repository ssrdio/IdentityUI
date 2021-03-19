﻿using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect
{
    public class ClientTokenEntity : OpenIddictEntityFrameworkCoreToken<string, ClientEntity, ClientConsentEntity>, IIdentityUIEntity
    {
    }
}
