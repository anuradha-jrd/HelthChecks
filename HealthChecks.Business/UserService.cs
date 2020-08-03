using HealthChecks.Business.Interfaces;
using HealthChecks.Business.Models;
using HealthChecks.DataAccess.Entities;
using HealthChecks.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business
{
    public class UserService : IUserService
    {
        private readonly ILogger<ServiceManager> logger;
        private IUserRepository userRepository;
        private IServiceProviderRepository serviceProviderRepository;

        public UserService(ILogger<ServiceManager> logger, IUserRepository userRepository, IServiceProviderRepository serviceProviderRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.serviceProviderRepository = serviceProviderRepository;
        }

        public async Task AddUser(string name, string email, string password)
        {
            User user = new User() { Name = name, Email = email, Password = password };
            List<User> usersToUpdate = new List<User>();
            usersToUpdate.Add(user);

            await userRepository.Upsert(usersToUpdate);
        }

        public async Task<List<ClientSubscriptions>> GetSubscriptions(string email)
        {
            List<ClientSubscriptions> clientSubscriptions = new List<ClientSubscriptions>();

            User user = await userRepository.GetUserByEmail(email);
            if (user == null)
                return clientSubscriptions;

            List<ServiceProvider> subscriptions = user.Subscriptions.Select(s => s.Service).ToList();
            

            foreach (ServiceProvider provider in subscriptions)
            {
                clientSubscriptions.Add(new ClientSubscriptions { Host = provider.Host, Port = provider.Port, PollingFrequency = provider.PollingFrequency, OutageStart = provider.OutageStart, OutageEnd = provider.OutageEnd });
            }

            return clientSubscriptions;
        }

        public async Task AddSubscription(string email, string host, int port, int pollingFreaquency, int gracePeriod)
        {
            ServiceProvider serviceProvider = await serviceProviderRepository.GetByHostAndPort(host, port);
            TimeSpan requestedPollingFrequency = GetTimeSpan(pollingFreaquency);

            // If service provider is not found add new one
            serviceProvider = serviceProvider == null ? new ServiceProvider() { Host = host, Port = port, PollingFrequency = requestedPollingFrequency } : serviceProvider;

            // If service provider is existing always update the service provider with the maximum polling freaquency
            serviceProvider.PollingFrequency = serviceProvider.PollingFrequency > requestedPollingFrequency? requestedPollingFrequency : serviceProvider.PollingFrequency;

            List<ServiceProvider> serviceProvidersToUpdate = new List<ServiceProvider>();
            serviceProvidersToUpdate.Add(serviceProvider);

            // Update database with service provider details
            await serviceProviderRepository.Upsert(serviceProvidersToUpdate);

            // Refreash service provider after inserting to map the subscription for user
            if (serviceProvider.Id == 0)
                serviceProvider = await serviceProviderRepository.GetByHostAndPort(host, port);

            // if service provider not found in the user, update the subscription
            User user = await userRepository.GetUserByEmail(email);
            if(user.Subscriptions.Where(s => s.Service.Id == serviceProvider.Id).Count() == 0)
            {
                ServiceSubscription serviceSubscription = new ServiceSubscription() { Service = serviceProvider, PollingFrequency = requestedPollingFrequency, GracePeriod = GetTimeSpan(gracePeriod) };
                user.Subscriptions.Add(serviceSubscription);
                List<User> usersToUpdate = new List<User>();
                usersToUpdate.Add(user);

                await userRepository.Upsert(usersToUpdate);
            }
        }

        public Task RemoveSubscription(string email, int serviceProviderId)
        {
            // TODO: Implement the function later. Not requested in the requirment spec
            return Task.CompletedTask;
        }

        private TimeSpan GetTimeSpan(int seconds)
        {
            int hours = seconds / 3600;
            int mins = (seconds - (hours * 3600)) / 60;
            int secs = seconds - (hours * 3600) - (mins * 60);
            return new TimeSpan(hours, mins, secs);
        }
    }
}
