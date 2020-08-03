using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.DataAccess.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<ServiceSubscription> Subscriptions { get; set; }
    }
}
