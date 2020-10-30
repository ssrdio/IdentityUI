using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Mvc
{
    public class AuditController : GroupAdminBaseController
    {
        private readonly IGroupAdminAuditDataService _groupAdminAuditDataService;

        public AuditController(IGroupAdminAuditDataService groupAdminAuditDataService)
        {
            _groupAdminAuditDataService = groupAdminAuditDataService;
        }

        public IActionResult Index()
        {
            AuditIndexViewModel viewModel = _groupAdminAuditDataService.GetIndexViewModel();

            return View(viewModel);
        }
    }
}
