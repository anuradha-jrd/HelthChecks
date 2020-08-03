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
    public class ServiceManager : IServiceManager
    {
        private readonly ILogger<ServiceManager> logger;
        private IServiceProviderRepository serviceProviderRepository;
        private IUserRepository userRepository;
        private INetworkService networkService;
        private IMailingService mailingService;

        public ServiceManager(ILogger<ServiceManager> logger, IServiceProviderRepository serviceProviderRepository, IUserRepository userRepository, INetworkService networkService, IMailingService mailingService)
        {
            this.logger = logger;
            this.serviceProviderRepository = serviceProviderRepository;
            this.userRepository = userRepository;
            this.networkService = networkService;
            this.mailingService = mailingService;
        }

        public async Task<TimeSpan> UpdateServiceHelth()
        {
            // Read saved service providers from the DB
            List<ServiceProvider> serviceProviders = await serviceProviderRepository.GetAll();

            // Check network connectivity status
            ServiceStatusResult updateResult = await networkService.CheckTcpConnection(serviceProviders);

            // write new status to the DB
            Task updateServiceProviderTask = serviceProviderRepository.Upsert(updateResult.ServiceProviders);

            // send mail notifications
            Task<TimeSpan> sendMailTask = SendNotifications(updateResult);

            // DB update, email notification and polling freaquency read can happen asynchronusly

            // Let the DB query run asynchronously while above two operations runs
            Task<TimeSpan> pollingFreaquencyTask = serviceProviderRepository.GetMaximumPollingFreaquency();

            // wait for DB update and notifications to complete
            await updateServiceProviderTask;

            TimeSpan minimumGracePeriod = await sendMailTask;

            // wait for the query result. This should be already completed
            TimeSpan pollingFreaquency = await pollingFreaquencyTask;

            // Next update cycle will be initialized based on the minimum value between polling frequency and minimum grace period of failed services.
            // From the NetworkService class correct polling frequency will be checked and skipped unnecessary network checks
            TimeSpan nextUpdateFrequency = minimumGracePeriod < pollingFreaquency ? minimumGracePeriod : pollingFreaquency;

            return nextUpdateFrequency;
        }

        private async Task<TimeSpan> SendNotifications(ServiceStatusResult statusResult)
        {
            List<Task> tasks = new List<Task>();
            List<TimeSpan> gracePeriods = new List<TimeSpan>();

            // Send service up notifications
            List<SubscriptionResults> serviceUpMailRecepients = await GetSubscribers(statusResult.WentOnline);
            foreach (SubscriptionResults recepient in serviceUpMailRecepients)
            {
                // TODO: Read text from config file
                tasks.Add(mailingService.Send(recepient.User.Email, "Service Up", GetMessageBody(recepient.Subscriptions.Select(s => s.Service).ToList())));
            }

            // Send service down notifications
            List<SubscriptionResults> serviceDownMailRecepients = await GetSubscribers(statusResult.WentOffline);
            foreach (SubscriptionResults recepient in serviceDownMailRecepients)
            {
                // TODO: Read text from config file
                tasks.Add(mailingService.Send(recepient.User.Email, "Service Down", GetMessageBody(recepient.Subscriptions.Select(s => s.Service).ToList())));
                gracePeriods.AddRange(recepient.Subscriptions.Select(s => s.GracePeriod));
            }

            await Task.WhenAll(tasks.ToArray());

            // Find the minimum grace period out of all subscriptions (excluding zeros)
            // If no entries found return a big value, so it will be ignored and polling freaquency will be considered
            var minimumGracePeriod = gracePeriods.Count > 0 ? gracePeriods.Min() : new TimeSpan(24,0,0);

            return minimumGracePeriod;
        }

        /// <summary>
        /// Get filtered list of users who subscribed for provided list of service providers
        /// </summary>
        /// <param name="serviceProviders">list of service providers to check against user subscriptions</param>
        /// <returns>Returns list of users and their subscriptions</returns>
        private async Task<List<SubscriptionResults>> GetSubscribers(List<ServiceProvider> serviceProviders)
        {
            List<int> serviceProviderIds = serviceProviders.Select(s => s.Id).ToList();
            // List<User> users = await userRepository.GetSubscribedUser(serviceProviderIds);
            List<User> users = await userRepository.GetAll();
            List<SubscriptionResults> subscriptionResults = new List<SubscriptionResults>();

            foreach (User user in users)
            {
                List<int> userSubscriptions = user.Subscriptions.Select(s => s.Service.Id).ToList();
                List<int> filteredProviders = serviceProviderIds.Intersect(userSubscriptions).ToList();

                if(filteredProviders.Count > 0)
                {
                    SubscriptionResults subscription = new SubscriptionResults();
                    subscription.User = user;

                    List<ServiceSubscription> subscriptions = user.Subscriptions.Where(s => filteredProviders.Contains(s.Service.Id)).ToList();

                    foreach (ServiceSubscription item in subscriptions)
                    {
                        // need to read the values from passed list as it may not yet written to database
                        ServiceProvider serviceProvider = serviceProviders.First(s => s.Id == item.Service.Id);

                        // if grace period is not defined, notify user
                        if(serviceProvider.Status == false && item.GracePeriod.TotalSeconds == 0)
                        {
                            subscription.Subscriptions.Add(item);
                        }
                        // if last failed time + grace period is less than current time, notify user
                        else if (serviceProvider.Status == false && serviceProvider.LastFailedTime.Add(item.GracePeriod) <= DateTime.Now)
                        {
                            subscription.Subscriptions.Add(item);
                        }
                        // if service went online, notify user regardless of the grace period
                        else
                        {
                            subscription.Subscriptions.Add(item);
                        }
                    }

                    subscriptionResults.Add(subscription);
                }
            }

            return subscriptionResults;
        }

        private string GetMessageBody(List<ServiceProvider> serviceProviders)
        {
            // TODO: Read text from config file
            string body = @"\nService | Status \n";
            foreach (ServiceProvider provider in serviceProviders)
            {
                body += string.Format(@"{0}:{1} | {2} \n", provider.Host, provider.Port, provider.Status);
            }

            return body;
        }
    }
}
