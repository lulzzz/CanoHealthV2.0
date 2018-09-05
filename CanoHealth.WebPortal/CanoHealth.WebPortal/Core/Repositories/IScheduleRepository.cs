using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        IEnumerable<Schedule> GetScheduleDetails(Guid? locationId = null, Guid? doctorId = null);
    }
}
