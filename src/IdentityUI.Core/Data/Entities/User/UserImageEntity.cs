using System;
using SSRD.IdentityUI.Core.Data.Entities.Identity;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public class UserImageEntity : IBaseEntity
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public long Id { get; set; }
        public byte[] BlobImage { get; set; }
        public string FileName { get; set; }

        public string UserId { get; set; }
        public virtual AppUserEntity User { get; set; }

        public string URL { get { return $"data:image/jpg;base64,{Convert.ToBase64String(BlobImage)}"; } }

        public UserImageEntity()
        {
        }

        public UserImageEntity(string userId, byte[] blobImage, string fileName)
        {
            UserId = userId;
            BlobImage = blobImage;
            FileName = fileName;
        }

        public UserImageEntity(long id)
        {
            Id = id;
        }
    }
}
