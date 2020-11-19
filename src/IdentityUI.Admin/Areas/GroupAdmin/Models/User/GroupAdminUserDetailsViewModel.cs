namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User
{
    public class GroupAdminUserDetailsViewModel : GroupAdminViewModel
    {
        public long GroupUserId { get; set; }

        public GroupAdminUserDetailsViewModel(string groupId, long groupUserId) : base(groupId)
        {
            GroupUserId = groupUserId;
        }
    }
}
