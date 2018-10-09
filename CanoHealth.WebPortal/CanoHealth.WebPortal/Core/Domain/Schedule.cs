using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Schedule
    {
        public Guid ScheduleId { get; set; }

        public DateTime Start { get; set; } //StartDateTime

        public DateTime End { get; set; }

        public string Title { get; set; }

        public String Description { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public bool IsAllDay { get; set; }

        public string RecurrenceRule { get; set; }

        public Guid? RecurrenceID { get; set; }

        public string RecurrenceException { get; set; }

        [StringLength(250)]
        public string StartTimezone { get; set; }

        [StringLength(250)]
        public string EndTimezone { get; set; }

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
            Start = schedule.Start;
            End = schedule.End;
            Description = schedule.Description;
            IsAllDay = schedule.IsAllDay;
            PlaceOfServiceId = schedule.PlaceOfServiceId;
            RecurrenceID = schedule.RecurrenceID;
            RecurrenceRule = schedule.RecurrenceRule;
            RecurrenceException = schedule.RecurrenceException;
            StartTimezone = schedule.StartTimezone;
            EndTimezone = schedule.EndTimezone;
        }
    }
}