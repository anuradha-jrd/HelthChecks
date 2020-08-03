using HealthChecks.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.Business.Models
{
    public class SubscriptionResults
    {
        public User User { get; set; }

        public List<ServiceSubscription> Subscriptions { get; set; }

        public SubscriptionResults()
        {
            Subscriptions = new List<ServiceSubscription>();
        }
    }
}
