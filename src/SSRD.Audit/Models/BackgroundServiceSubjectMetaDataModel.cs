using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class BackgroundServiceSubjectMetaDataModel
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }

        public BackgroundServiceSubjectMetaDataModel(string userId, string groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }
    }
}
