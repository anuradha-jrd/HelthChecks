using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
        Task<List<int>> Add(List<T> items);

        Task Upsert(List<T> items);

        Task Delete(List<T> items);

        Task<T> GetById(int id);

        Task<List<T>> GetAll();
    }
}
