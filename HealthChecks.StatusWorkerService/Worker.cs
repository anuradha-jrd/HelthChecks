using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HealthChecks.Business;
using HealthChecks.Business.Interfaces;
using HealthChecks.DataAccess.Interfaces;
using HealthChecks.DataAccess.Repositories;
using HealthChecks.DataAccess.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthChecks.StatusWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceScopeFactory scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    IServiceManager serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                    // update service health and get the next time to execute the update
                    TimeSpan pollingFreaquency = await serviceManager.UpdateServiceHelth();
                    int pollingDelay = Convert.ToInt32(pollingFreaquency.TotalMilliseconds);

                    // Final round of sanity test to avoid any malicious values. Update is restricted to minimum 1 second
                    pollingDelay = pollingDelay > 1000 ? pollingDelay : 1000;

                    // Keep executing above code after the given delay
                    await Task.Delay(pollingDelay, stoppingToken);
                }
            }
        }
    }
}
