using HealthChecks.Business.Interfaces;
using HealthChecks.Business.Models;
using HealthChecks.DataAccess.Entities;
using HealthChecks.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace HealthChecks.Business.Test
{
    [TestClass]
    public class ServiceManagerTests
    {
        [TestMethod]
        public void UpdateServiceHealth_Should_Return_Polling_Freaquency()
        {
            var mockLogger = new Mock<ILogger<ServiceManager>>();
            var mockServiceProviderRepo = new Mock<IServiceProviderRepository>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockNetworkService = new Mock<INetworkService>();
            var mockMailingService = new Mock<IMailingService>();

            TimeSpan maximumPollingFreaquency = new TimeSpan(0, 0, 1);
            TimeSpan minimumGracePeriod = new TimeSpan(1, 0, 0);

            ServiceProvider serviceProvider = new ServiceProvider() { Host = "127.0.0.1", Port = 80, PollingFrequency = maximumPollingFreaquency };
            
            List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
            serviceProviders.Add(serviceProvider);

            List<ServiceSubscription> serviceSubscriptions = new List<ServiceSubscription>();
            serviceSubscriptions.Add(new ServiceSubscription() { Service = serviceProvider, GracePeriod = minimumGracePeriod, PollingFrequency = maximumPollingFreaquency });

            List<User> users = new List<User>();
            users.Add(new User() { Name = "abc", Email = "abc@abc.com", Subscriptions = serviceSubscriptions });

            ServiceStatusResult updateResult = new ServiceStatusResult();
            updateResult.WentOffline.Add(serviceProvider);

            mockNetworkService.Setup(n => n.CheckTcpConnection(serviceProviders)).ReturnsAsync(updateResult);
            mockUserRepo.Setup(u => u.GetAll()).ReturnsAsync(users);
            mockServiceProviderRepo.Setup(s => s.GetMaximumPollingFreaquency()).ReturnsAsync(maximumPollingFreaquency);

            ServiceManager serviceManager = new ServiceManager(mockLogger.Object, mockServiceProviderRepo.Object, mockUserRepo.Object, mockNetworkService.Object, mockMailingService.Object);

            // execute method
            TimeSpan pollingFreaquencyResult = serviceManager.UpdateServiceHelth().Result;

            // UpdateServiceHelth() should return the value we configured for minimum polling freaquency as it's the lowest value
            Assert.AreEqual(pollingFreaquencyResult, maximumPollingFreaquency);
        }

        [TestMethod]
        public void UpdateServiceHealth_Should_Return_grace_period()
        {
            var mockLogger = new Mock<ILogger<ServiceManager>>();
            var mockServiceProviderRepo = new Mock<IServiceProviderRepository>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockNetworkService = new Mock<INetworkService>();
            var mockMailingService = new Mock<IMailingService>();

            // This means in one failed service provider grace period is less than the next polling time
            // in this case minimum grace period should return as the next polling time
            TimeSpan maximumPollingFreaquency = new TimeSpan(1, 0, 1);
            TimeSpan minimumGracePeriod = new TimeSpan(0, 0, 1);

            ServiceProvider serviceProvider = new ServiceProvider() { Host = "127.0.0.1", Port = 80, PollingFrequency = maximumPollingFreaquency };

            List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
            serviceProviders.Add(serviceProvider);

            List<ServiceSubscription> serviceSubscriptions = new List<ServiceSubscription>();
            serviceSubscriptions.Add(new ServiceSubscription() { Service = serviceProvider, GracePeriod = minimumGracePeriod, PollingFrequency = maximumPollingFreaquency });

            List<User> users = new List<User>();
            users.Add(new User() { Name = "abc", Email = "abc@abc.com", Subscriptions = serviceSubscriptions });

            ServiceStatusResult updateResult = new ServiceStatusResult();
            updateResult.WentOffline.Add(serviceProvider);

            mockNetworkService.Setup(n => n.CheckTcpConnection(serviceProviders)).ReturnsAsync(updateResult);
            mockUserRepo.Setup(u => u.GetAll()).ReturnsAsync(users);
            mockServiceProviderRepo.Setup(s => s.GetMaximumPollingFreaquency()).ReturnsAsync(maximumPollingFreaquency);

            ServiceManager serviceManager = new ServiceManager(mockLogger.Object, mockServiceProviderRepo.Object, mockUserRepo.Object, mockNetworkService.Object, mockMailingService.Object);

            // execute method
            TimeSpan pollingFreaquencyResult = serviceManager.UpdateServiceHelth().Result;

            // UpdateServiceHelth() should return the value we configured for minimum grace period as it's the lowest value
            Assert.AreEqual(pollingFreaquencyResult, minimumGracePeriod);
        }
    }
}
