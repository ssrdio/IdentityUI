using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityUI.Dev.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUI.Dev.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DevController : ControllerBase
    {
        private readonly UserSeeder userSeeder;

        public DevController(UserSeeder userSeeder)
        {
            this.userSeeder = userSeeder;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateUsersForGraph()
        {
            await userSeeder.Seed(new DateTime(2020, 5, 1), new DateTime(2020, 5, 2));

            return Ok();
        }
    }
}