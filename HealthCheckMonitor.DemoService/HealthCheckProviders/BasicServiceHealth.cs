using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.DemoService.HealthCheckProviders
{
    public class BasicServiceHealth : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<HealthCheckResult>(HealthCheckResult.Healthy());
        }
    }
}
