using HealthChecks.DataAccess.Entities;
using HealthChecks.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IHealthCheckContext unitOfWork) : base(unitOfWork) { }

        protected override void AddItem(User item)
        {
            unitOfWork.GetUsers().Add(item);
        }

        protected override void AddRange(List<User> items)
        {
            unitOfWork.GetUsers().AddRange(items);
        }

        protected override IQueryable<User> GetDataSet()
        {
            return unitOfWork.GetUsers();
        }

        protected override void RemoveRange(List<User> items)
        {
            unitOfWork.GetUsers().RemoveRange(items);
        }

        protected override User UpdateItem(User target, User source)
        {
            target.Name = source.Name;
            target.Email = source.Email;
            target.Password = source.Password;
            target.Subscriptions = source.Subscriptions;
            target.UpdatedDate = DateTime.Now;

            return target;
        }

        /// <summary>
        /// Gets a list of users who got subscribed for a given set of service provider ids
        /// </summary>
        /// <param name="ids">list of service provider ids</param>
        /// <returns>returns list of filtered users by their subscription</returns>
        public Task<List<User>> GetSubscribedUser(List<int> ids)
        {
            return Task.Factory.StartNew(() =>
            {
                return unitOfWork.GetUsers().Where(user => user.Subscriptions.Select(s => s.Id).Intersect(ids).Count() > 0).ToList();
            });
        }

        public Task<User> GetUserByEmail(string email)
        {
            return Task.Factory.StartNew(() =>
            {
                return unitOfWork.GetUsers().FirstOrDefault(user => user.Email == email);
            });
        }
    }
}
