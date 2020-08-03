using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.Business;
using HealthChecks.Business.Interfaces;
using HealthChecks.DataAccess.Interfaces;
using HealthChecks.DataAccess.Repositories;
using HealthChecks.DataAccess.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HealthChecks.StatusWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<HealthCheckContext>();
                    optionsBuilder.UseInMemoryDatabase("HealthChecksDb");
                    services.AddScoped<IHealthCheckContext>(s => new HealthCheckContext(optionsBuilder.Options));

                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
                    services.AddScoped<INetworkService, NetworkService>();
                    services.AddScoped<IMailingService, MailingService>();

                    services.AddScoped<IServiceManager, ServiceManager>();

                    services.AddHostedService<Worker>();
                });
    }
}
