using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Schedule
    {
        public Guid ScheduleId { get; set; }

        public string Title { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string StartTimezone { get; set; }

        public string EndTimeZone { get; set; }

        public String Description { get; set; }

        public bool IsAllDay { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        //Navegation Properties
        public PlaceOfService Location { get; set; }

        public ICollection<DoctorSchedule> DoctorSchedules { get; set; }

        public Schedule()
        {
            DoctorSchedules = new Collection<DoctorSchedule>();
        }

        /*
         public int MeetingID { get; set; }
        public string Description { get; set; }
        public DateTime End { get; set; }
        public string EndTimezone { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceException { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RoomID { get; set; }
        public DateTime Start { get; set; }
        public string StartTimezone { get; set; }
        public string Title { get; set; }

        public virtual ICollection<MeetingAttendee> MeetingAttendees { get; set; }
        public virtual Meeting Recurrence { get; set; }
        public virtual ICollection<Meeting> InverseRecurrence { get; set; }
         
         */
    }
}