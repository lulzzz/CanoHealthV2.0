using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Schedule GetDetailedSchedule(Guid scheduleId);

        IEnumerable<Schedule> GetSchedules();
    }
}
