using System;
using System.Threading.Tasks;
using IdentityUI.Dev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace IdentityUI.Dev.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DevController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public DevController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateUsersForGraph()
        {
            UserSeeder userSeeder = _serviceProvider.GetRequiredService<UserSeeder>();

            await userSeeder.Seed(new DateTime(2020, 5, 1), new DateTime(2020, 5, 2));

            return Ok();
        }
    }
}