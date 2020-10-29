using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Models
{
    public class SubjectMetadataModel
    {
        public string ImpersonatorId { get; set; }

        public SubjectMetadataModel()
        {
        }

        public SubjectMetadataModel(string impersonatorId)
        {
            ImpersonatorId = impersonatorId;
        }
    }
}
