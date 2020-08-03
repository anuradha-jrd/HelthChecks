using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.Business.Interfaces;
using HealthChecks.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HealthChecks.ClientService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ILogger<SubscriptionController> logger;
        private IUserService userService;

        public SubscriptionController(ILogger<SubscriptionController> logger, IUserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<ClientSubscriptions>> Get(string email)
        {
            // TODO: sanitize input

            return await userService.GetSubscriptions(email);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string email, string host, int port, int pollingFreaquency, int gracePeriod)
        {
            // TODO: sanitize input

            await userService.AddSubscription(email, host, port, pollingFreaquency, gracePeriod);
            return Ok();
        }
    }
}
