using Microsoft.AspNetCore.Mvc;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Dashboard.Components.Cards
{
    public class CardsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

        public class ViewModel
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }

            public ViewModel(string roleId, string roleName)
            {
                RoleId = roleId;
                RoleName = roleName;
            }
        }
    }
}
