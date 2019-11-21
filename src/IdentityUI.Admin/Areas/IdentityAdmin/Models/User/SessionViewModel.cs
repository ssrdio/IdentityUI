using SSRD.IdentityUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class SessionViewModel
    {
        public long Id { get; set; }
        public string Ip { get; set; }

        private DateTimeOffset? CreatedDateTime { get; set; }
        private DateTimeOffset LastAccessDateTime { get; set; }

        public string Created { get { return CreatedDateTime.HasValue ? CreatedDateTime.Value.ToString(DateTimeFormats.DEFAULT_DATE_TIME_FORMAT) : null; } }
        public string LastAccess { get { return LastAccessDateTime.ToString(DateTimeFormats.DEFAULT_DATE_TIME_FORMAT); } }

        public SessionViewModel(long id, string ip, DateTimeOffset? created, DateTimeOffset lastAccess)
        {
            Id = id;
            Ip = ip;
            CreatedDateTime = created;
            LastAccessDateTime = lastAccess;
        }
    }
}
