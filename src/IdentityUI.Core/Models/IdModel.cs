using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models
{
    public class IdModel<TValue>
    {
        public TValue Id { get; set; }

        public IdModel(TValue id)
        {
            Id = id;
        }
    }
}
