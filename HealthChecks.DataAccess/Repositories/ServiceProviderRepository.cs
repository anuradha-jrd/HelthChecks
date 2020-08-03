using HealthChecks.DataAccess.Entities;
using HealthChecks.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Repositories
{
    public class ServiceProviderRepository : BaseRepository<ServiceProvider>, IServiceProviderRepository
    {
        public ServiceProviderRepository(IHealthCheckContext unitOfWork) : base(unitOfWork) { }

        protected override void AddItem(ServiceProvider item)
        {
            unitOfWork.GetServices().Add(item);
        }

        protected override void AddRange(List<ServiceProvider> items)
        {
            unitOfWork.GetServices().AddRange(items);
        }

        protected override IQueryable<ServiceProvider> GetDataSet()
        {
            return unitOfWork.GetServices();
        }

        protected override void RemoveRange(List<ServiceProvider> items)
        {
            unitOfWork.GetServices().RemoveRange(items);
        }

        protected override ServiceProvider UpdateItem(ServiceProvider target, ServiceProvider source)
        {
            target.Host = source.Host;
            target.Port = source.Port;
            target.PollingFrequency = source.PollingFrequency;
            target.LastFailedTime = source.LastFailedTime;
            target.OutageStart = source.OutageStart;
            target.OutageEnd = source.OutageEnd;
            target.UpdatedDate = DateTime.Now;

            return target;
        }

        /// <summary>
        /// Gets the maximum polling frequency by selecting the minimum timespan value
        /// </summary>
        /// <returns>Returns a timespan</returns>
        public Task<TimeSpan> GetMaximumPollingFreaquency()
        {
            // exclude zeros to be on the safer side
            IQueryable<ServiceProvider> filtered = unitOfWork.GetServices().Where(r => r.PollingFrequency.TotalSeconds > 0);
            
            // if no services available return the defauld value
            if (filtered.Count() == 0)
                return Task.FromResult(new TimeSpan(0, 0, 3));

            TimeSpan freaquency = filtered.Select(s => s.PollingFrequency).Min();
            return Task.FromResult<TimeSpan>(freaquency);
        }

        /// <summary>
        /// Get the Service provider object by host and port
        /// </summary>
        /// <param name="host">the host</param>
        /// <param name="port">the port</param>
        /// <returns>returns a service provider object</returns>
        public async Task<ServiceProvider> GetByHostAndPort(string host, int port)
        {
            return await unitOfWork.GetServices().FirstOrDefaultAsync(s => s.Host == host && s.Port == port);
        }
    }
}
