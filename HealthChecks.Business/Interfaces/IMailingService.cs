using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecks.Business.Interfaces
{
    public interface IMailingService
    {
        Task Send(string recepient, string subject, string message);
    }
}
