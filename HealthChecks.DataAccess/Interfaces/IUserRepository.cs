using HealthChecks.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetSubscribedUser(List<int> ids);

        Task<User> GetUserByEmail(string email);
    }
}
