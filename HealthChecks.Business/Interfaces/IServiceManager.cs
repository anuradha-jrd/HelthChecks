using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business.Interfaces
{
    public interface IServiceManager
    {
        Task<TimeSpan> UpdateServiceHelth();
    }
}
