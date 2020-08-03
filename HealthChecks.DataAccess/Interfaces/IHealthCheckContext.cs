using HealthChecks.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Interfaces
{
    public interface IHealthCheckContext
    {
        DbSet<ServiceProvider> GetServices();

        DbSet<User> GetUsers();

        DbSet<ServiceSubscription> GetSubscriptions();

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
