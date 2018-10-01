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
        public Guid ScheduleId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        private DateTime start;
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

        public int? RecurrenceID { get; set; }

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
                Description = schedule.Description,
                Start = schedule.StartDateTime,
                End = schedule.EndDateTime,
                StartTimezone = schedule.StartTimezone,
                EndTimezone = schedule.EndTimeZone,
                IsAllDay = schedule.IsAllDay,
                LocationId = schedule.PlaceOfServiceId,
                Doctors = schedule.DoctorSchedules.Select(d => d.DoctorId)
            };
        }

        public Schedule ConvertToSchedule()
        {
            return new Schedule
            {
                ScheduleId = this.ScheduleId != Guid.Empty ? this.ScheduleId : Guid.NewGuid(),
                Title = this.Title,
                Description = this.Description,
                IsAllDay = this.IsAllDay,
                StartDateTime = this.Start,
                EndDateTime = this.End,
                StartTimezone = this.StartTimezone,
                EndTimeZone = this.EndTimezone,
                PlaceOfServiceId = this.LocationId
            };
        }
    }
}