using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using CanoHealth.WebPortal.ViewModels;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class DoctorScheduleRepository : Repository<DoctorSchedule>, IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<DoctorSchedule> GetDoctorSchedules(ScheduleViewModel schedule)
        {
            var doctorschedulesfound = new List<DoctorSchedule>();

            if (schedule.IsAllDay)
            {
                var nextday = schedule.Start.AddDays(1);
                doctorschedulesfound = EnumarableGetAll(
                                filter: ds => ds.ScheduleId != schedule.ScheduleId &&
                                ds.Schedule.StartDateTime >= schedule.Start &&
                                ds.Schedule.StartDateTime < nextday &&
                                schedule.Doctors.Contains(ds.DoctorId),
                                includeProperties: new Expression<Func<DoctorSchedule, object>>[]
                                {
                                    s => s.Schedule,
                                    d => d.Doctor
                                }).ToList();
            }
            else
            {
                doctorschedulesfound = EnumarableGetAll(
                               filter: ds => ds.ScheduleId != schedule.ScheduleId &&
                               ds.Schedule.StartDateTime <= schedule.Start &&
                               ds.Schedule.EndDateTime > schedule.Start &&
                               schedule.Doctors.Contains(ds.DoctorId),
                               includeProperties: new Expression<Func<DoctorSchedule, object>>[]
                               {
                                    s => s.Schedule,
                                    d => d.Doctor
                               }).ToList();
            }

            return doctorschedulesfound;
        }
    }
}