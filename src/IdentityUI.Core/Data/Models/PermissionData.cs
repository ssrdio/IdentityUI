namespace SSRD.IdentityUI.Core.Data.Models
{
    public class PermissionData
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public PermissionData(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
