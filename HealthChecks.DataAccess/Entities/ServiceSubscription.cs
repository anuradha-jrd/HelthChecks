using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.DataAccess.Entities
{
    public class ServiceSubscription : BaseEntity
    {
        public ServiceProvider Service { get; set; }

        public TimeSpan PollingFrequency { get; set; }

        public TimeSpan GracePeriod { get; set; }
    }
}
