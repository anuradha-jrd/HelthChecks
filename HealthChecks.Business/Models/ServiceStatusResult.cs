using HealthChecks.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.Business.Models
{
    public class ServiceStatusResult
    {
        public List<ServiceProvider> ServiceProviders { get; set; }

        public List<ServiceProvider> WentOffline { get; set; }

        public List<ServiceProvider> WentOnline { get; set; }

        public ServiceStatusResult()
        {
            ServiceProviders = new List<ServiceProvider>();
            WentOffline = new List<ServiceProvider>();
            WentOnline = new List<ServiceProvider>();
        }
    }
}
