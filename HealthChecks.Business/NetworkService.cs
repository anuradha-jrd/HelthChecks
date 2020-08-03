using HealthChecks.Business.Interfaces;
using HealthChecks.Business.Models;
using HealthChecks.DataAccess.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business
{
    public class NetworkService : INetworkService
    {
        private readonly ILogger<NetworkService> logger;
        protected ServiceStatusResult serviceStatusResult;

        public NetworkService(ILogger<NetworkService> logger)
        {
            this.logger = logger;
            serviceStatusResult = new ServiceStatusResult();
        }

        public async Task<ServiceStatusResult> CheckTcpConnection(List<ServiceProvider> tcpServiceProviders)
        {
            // List<Task> tasks = new List<Task>();
            serviceStatusResult = new ServiceStatusResult();

            foreach (ServiceProvider tcpServiceProvider in tcpServiceProviders)
            {
                if (tcpServiceProvider.UpdatedDate.Add(tcpServiceProvider.PollingFrequency) <= DateTime.Now // If polling is due and
                    && (tcpServiceProvider.OutageStart > DateTime.Now && tcpServiceProvider.OutageEnd >= DateTime.Now // If outage starts and ends in a future date, check connectivity
                    || tcpServiceProvider.OutageStart < DateTime.Now && tcpServiceProvider.OutageEnd < DateTime.Now)) // If outage started and ended in a past date, check connectivity
                {
                    await CheckTcpConnection(tcpServiceProvider);
                }
            }

            return serviceStatusResult;
        }

        public async Task<ServiceProvider> CheckTcpConnection(ServiceProvider tcpServiceProvider)
        {
            using (var tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(tcpServiceProvider.Host, tcpServiceProvider.Port);

                bool status = tcpClient.Connected;

                if(tcpServiceProvider.Status == true && status == false)
                {
                    tcpServiceProvider.LastFailedTime = DateTime.Now;
                    serviceStatusResult.WentOffline.Add(tcpServiceProvider);
                }
                else if (tcpServiceProvider.Status == false && status == true)
                {
                    serviceStatusResult.WentOnline.Add(tcpServiceProvider);
                }

                tcpServiceProvider.Status = tcpClient.Connected;

                serviceStatusResult.ServiceProviders.Add(tcpServiceProvider);
            }

            return tcpServiceProvider;
        }
    }
}
