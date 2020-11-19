namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Settings
{
    public class GroupAdminSettingsDetailsModel
    {
        public string GroupName { get; set; }

        public GroupAdminSettingsDetailsModel(string groupName)
        {
            GroupName = groupName;
        }
    }
}
