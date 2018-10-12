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

        public IEnumerable<Schedule> GetSchedules()
        {
            var schedules = EnumarableGetAll(
                includeProperties: new Expression<Func<Schedule, object>>[] {
                    s => s.DoctorSchedules.Select(d => d.Doctor)
                }).ToList();

            return schedules;
        }

        public Schedule GetDetailedSchedule(Guid scheduleId)
        {
            var schedule = EnumarableGetAll(
                filter: s => s.ScheduleId == scheduleId,
                includeProperties: new Expression<Func<Schedule, object>>[] {
                    l => l.Location,
                    s => s.DoctorSchedules.Select(d => d.Doctor)
                }).FirstOrDefault();

            return schedule;
        }
    }
}