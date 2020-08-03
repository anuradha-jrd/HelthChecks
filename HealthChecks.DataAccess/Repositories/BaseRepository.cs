using HealthChecks.DataAccess.Entities;
using HealthChecks.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.DataAccess.Repositories
{
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        protected IHealthCheckContext unitOfWork;
        protected CancellationToken cancellationToken;

        public BaseRepository(IHealthCheckContext unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            cancellationToken = CancellationToken.None;
        }

        public async Task<List<int>> Add(List<T> items)
        {
            AddRange(items);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);
            IEnumerable<int> itemIds = items.Select(i => i.Id);
            return await Task.Factory.StartNew(() => itemIds.ToList());
        }

        public async Task Delete(List<T> items)
        {
            RemoveRange(items);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<T>> GetAll()
        {
            return await GetDataSet().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            T record = await GetDataSet().SingleOrDefaultAsync(i => i.Id == id);
            return record;
        }

        public async Task Upsert(List<T> items)
        {
            foreach (T item in items)
            {
                if (item.Id == 0)
                {
                    AddItem(item);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    T itemToUpdate = GetDataSet().FirstOrDefault(i => i.Id == item.Id);
                    itemToUpdate = UpdateItem(itemToUpdate, item);
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    await this.unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }

        protected abstract void AddItem(T item);

        protected abstract void AddRange(List<T> items);

        protected abstract void RemoveRange(List<T> items);

        protected abstract T UpdateItem(T target, T source);

        protected abstract IQueryable<T> GetDataSet();
    }
}
