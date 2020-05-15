using SSRD.IdentityUI.Core.Data.Entities;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class PermissionSeedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public PermissionSeedModel(string name, string description)
        {
            Name = name;
            Description = description;
        }

        internal PermissionEntity ToEntity()
        {
            return new PermissionEntity(
                name: Name,
                description: Description);
        }
    }
}
