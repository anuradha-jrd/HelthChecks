using HealthChecks.Business.Models;
using HealthChecks.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business.Interfaces
{
    public interface INetworkService
    {
        Task<ServiceStatusResult> CheckTcpConnection(List<ServiceProvider> tcpServiceProviders);

        Task<ServiceProvider> CheckTcpConnection(ServiceProvider tcpServiceProvider);
    }
}
