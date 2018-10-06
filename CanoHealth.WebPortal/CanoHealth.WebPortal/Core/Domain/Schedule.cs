using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Schedule
    {
        public Guid ScheduleId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Title { get; set; }

        public String Description { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public bool IsAllDay { get; set; }

        public string RecurrenceRule { get; set; }

        public Guid? RecurrenceID { get; set; }

        public string RecurrenceException { get; set; }

        public string StartTimezone { get; set; }

        public string EndTimeZone { get; set; }

        //Navegation Properties
        public PlaceOfService Location { get; set; }

        public ICollection<DoctorSchedule> DoctorSchedules { get; set; }

        public ICollection<Schedule> Schedules1 { get; set; }

        public Schedule Schedule1 { get; set; }

        public Schedule()
        {
            DoctorSchedules = new Collection<DoctorSchedule>();
            Schedules1 = new Collection<Schedule>();
        }

        public void Modify(Schedule schedule)
        {
            Title = schedule.Title;
            StartDateTime = schedule.StartDateTime;
            EndDateTime = schedule.EndDateTime;
            Description = schedule.Description;
            IsAllDay = schedule.IsAllDay;
            PlaceOfServiceId = schedule.PlaceOfServiceId;
            RecurrenceID = schedule.RecurrenceID;
            RecurrenceRule = schedule.RecurrenceRule;
            RecurrenceException = schedule.RecurrenceException;
            StartTimezone = schedule.StartTimezone;
            EndTimeZone = schedule.EndTimeZone;
        }
    }
}