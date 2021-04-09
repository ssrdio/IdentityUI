using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Session
{
    public class SessionModel
    {
        public long Id { get; set; }
        
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Os { get; set; }
        public string Device { get; set; }

        public DateTime LastAccess { get; set; }
        public DateTime Created { get; set; }

        public SessionModel()
        {
        }

        public SessionModel(long id, string ip, string userAgent, string os, string device, DateTime lastAccess, DateTime created)
        {
            Id = id;
            Ip = ip;
            UserAgent = userAgent;
            Os = os;
            Device = device;
            LastAccess = lastAccess;
            Created = created;
        }
    }
}
