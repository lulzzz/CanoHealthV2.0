using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        IEnumerable<DoctorSchedule> GetDoctorSchedules(ScheduleViewModel schedule);
    }
}
