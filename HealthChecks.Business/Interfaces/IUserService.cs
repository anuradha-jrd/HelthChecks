using HealthChecks.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business.Interfaces
{
    public interface IUserService
    {
        Task AddUser(string name, string email, string password);

        Task<List<ClientSubscriptions>> GetSubscriptions(string email);

        Task AddSubscription(string email, string host, int port, int pollingFreaquency, int gracePeriod);

        Task RemoveSubscription(string email, int serviceProviderId);
    }
}
