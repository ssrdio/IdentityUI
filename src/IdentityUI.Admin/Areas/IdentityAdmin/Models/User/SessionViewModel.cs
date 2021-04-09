using System;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class SessionViewModel
    {
        public long Id { get; set; }
        public string Ip { get; set; }

        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset LastAccess { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string FullUserAgent { get; set; }

        public string UserAgent { get; set; }
        public string Os { get; set; }
        public string Device { get; set; }

        public SessionViewModel(long id, string ip, DateTimeOffset? created, DateTimeOffset lastAccess, string fullUserAgent)
        {
            Id = id;
            Ip = ip;
            Created = created;
            LastAccess = lastAccess;
            FullUserAgent = fullUserAgent;
        }
    }
}
