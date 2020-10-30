using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Mvc
{
    public class DashboardController : GroupAdminBaseController
    {
        private readonly IGroupAdminDashboardService _groupAdminDashboardService;

        public DashboardController(IGroupAdminDashboardService groupAdminDashboardService)
        {
            _groupAdminDashboardService = groupAdminDashboardService;
        }

        [HttpGet("/[area]")]
        [HttpGet("/[area]/[controller]")]
        public async Task<IActionResult> Index()
        {
            GroupedStatisticsViewModel viewModel = await _groupAdminDashboardService.GetIndexViewModel();

            return View(viewModel);
        }
    }
}
