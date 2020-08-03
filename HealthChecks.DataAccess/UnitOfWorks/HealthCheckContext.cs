using HealthChecks.DataAccess.Entities;
using HealthChecks.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.DataAccess.UnitOfWorks
{
    public class HealthCheckContext : DbContext, IHealthCheckContext
    {
        public HealthCheckContext(DbContextOptions<HealthCheckContext> options) : base(options) { }

        public DbSet<ServiceProvider> Services { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ServiceSubscription> Subscriptions { get; set; }

        public DbSet<ServiceProvider> GetServices()
        {
            return this.Services;
        }

        public DbSet<ServiceSubscription> GetSubscriptions()
        {
            return this.Subscriptions;
        }

        public DbSet<User> GetUsers()
        {
            return this.Users;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // In memory DB will be used for demo purposes
            optionsBuilder.UseInMemoryDatabase("HealthChecksDb");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
