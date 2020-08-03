using HealthChecks.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Interfaces
{
    public interface IServiceProviderRepository : IRepository<ServiceProvider>
    {
        Task<TimeSpan> GetMaximumPollingFreaquency();

        Task<ServiceProvider> GetByHostAndPort(string host, int port);
    }
}
