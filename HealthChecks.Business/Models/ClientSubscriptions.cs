using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.Business.Models
{
    public class ClientSubscriptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public TimeSpan PollingFrequency { get; set; }

        public DateTime OutageStart { get; set; }

        public DateTime OutageEnd { get; set; }
    }
}
