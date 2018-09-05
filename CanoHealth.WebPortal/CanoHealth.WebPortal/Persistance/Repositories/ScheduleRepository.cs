using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Schedule> GetScheduleDetails(Guid? locationId = null, Guid? doctorId = null)
        {
            var schedules = EnumarableGetAll(
                includeProperties: new Expression<Func<Schedule, object>>[] {
                    s => s.DoctorSchedules.Select(d => d.Doctor)
                }).ToList();

            if (locationId != null)
                schedules = schedules.Where(l => l.PlaceOfServiceId == locationId).ToList();

            //if(doctorId != null)
            //    schedules = schedules.Where(ds => ds.DoctorSchedules.)

            return schedules;
        }
    }
}