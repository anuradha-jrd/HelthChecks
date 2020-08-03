using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.DataAccess.Entities
{
    public class BaseEntity
    {
        public virtual int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public BaseEntity()
        {
            CreatedDate = UpdatedDate = DateTime.Now;
        }
    }
}
