using CanoHealth.WebPortal.Core.Domain;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CanoHealth.WebPortal.ViewModels
{
    public class ScheduleViewModel : ISchedulerEvent
    {
        public Guid? ScheduleId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        private DateTime start;
        [Required]
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
            }
        }

        [Display(Name = "Time Zone")]
        public string StartTimezone { get; set; }

        private DateTime end;

        [Required]
        //[DateGreaterThan(OtherField = "Start")]
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
            }
        }

        public string EndTimezone { get; set; }

        [Display(Name = "Repeat")]
        public string RecurrenceRule { get; set; }

        public Guid? RecurrenceID { get; set; }

        public string RecurrenceException { get; set; }

        [Display(Name = "All day event")]
        public bool IsAllDay { get; set; }

        public string Timezone { get; set; }

        [Required]
        [Display(Name = "Location")]
        public Guid LocationId { get; set; }

        [Required]
        public IEnumerable<Guid> Doctors { get; set; }

        public ScheduleViewModel()
        {
            Doctors = new List<Guid>();
        }

        public static ScheduleViewModel Wrap(Schedule schedule)
        {
            return new ScheduleViewModel
            {
                ScheduleId = schedule.ScheduleId,
                Title = schedule.Title,
                Start = schedule.Start,
                End = schedule.End,
                StartTimezone = schedule.StartTimezone,
                EndTimezone = schedule.EndTimezone,
                Description = schedule.Description,
                IsAllDay = schedule.IsAllDay,
                LocationId = schedule.PlaceOfServiceId,
                RecurrenceRule = schedule.RecurrenceRule,
                RecurrenceException = schedule.RecurrenceException,
                RecurrenceID = schedule.RecurrenceID,
                Doctors = schedule.DoctorSchedules.Select(d => d.DoctorId),
                Timezone = "Etc/UTC"
            };
        }

        public Schedule ConvertToSchedule()
        {
            return new Schedule
            {
                ScheduleId = ScheduleId ?? Guid.NewGuid(),
                Title = Title,
                Start = Start,
                StartTimezone = StartTimezone,
                End = End,
                EndTimezone = EndTimezone,
                Description = Description,
                IsAllDay = IsAllDay,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                PlaceOfServiceId = LocationId
            };
        }
    }
}