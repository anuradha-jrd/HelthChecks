using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.DataAccess.Entities
{
    public class ServiceProvider : BaseEntity
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool Status { get; set; }

        public TimeSpan PollingFrequency { get; set; }

        public DateTime LastFailedTime { get; set; }

        public DateTime OutageStart { get; set; }

        public DateTime OutageEnd { get; set; }
    }
}
