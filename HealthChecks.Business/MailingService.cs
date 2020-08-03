using HealthChecks.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business
{
    public class MailingService : IMailingService
    {
        private readonly ILogger<MailingService> logger;

        public MailingService(ILogger<MailingService> logger)
        {
            this.logger = logger;
        }

        public Task Send(string recepient, string subject, string message)
        {
            logger.LogInformation("Sending mail to {0} - {1}", recepient, subject);

            // TODO: Implement mail send function
            return Task.CompletedTask;
        }
    }
}
