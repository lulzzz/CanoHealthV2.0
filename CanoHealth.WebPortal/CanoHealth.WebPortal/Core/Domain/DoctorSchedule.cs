using System;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorSchedule
    {
        public Guid DoctorScheduleId { get; set; }

        public Guid ScheduleId { get; set; }

        public Guid DoctorId { get; set; }

        //navegation properties
        public Schedule Schedule { get; set; }

        public Doctor Doctor { get; set; }
    }
}