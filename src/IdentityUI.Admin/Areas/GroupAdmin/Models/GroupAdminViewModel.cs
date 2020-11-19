namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models
{
    public class GroupAdminViewModel
    {
        public string GroupId { get; set; }

        public GroupAdminViewModel(string groupId)
        {
            GroupId = groupId;
        }
    }
}
