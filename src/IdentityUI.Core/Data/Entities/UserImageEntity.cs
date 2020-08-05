using System;
using SSRD.IdentityUI.Core.Data.Entities.Identity;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public class UserImageEntity : IBaseEntity
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public int Id { get; set; }
        public string UserId { get; set; }
        public byte[] BlobImage { get; set; }
        public string FileName { get; set; }
        public bool? IsDefault { get; set; }

        public virtual AppUserEntity User { get; set; }

        public string URL { get { return $"data:image/jpg;base64,{Convert.ToBase64String(BlobImage)}"; } }

        public UserImageEntity()
        {
        }
    }
}
