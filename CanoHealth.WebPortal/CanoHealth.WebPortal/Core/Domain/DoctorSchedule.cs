using System;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorSchedule
    {
        public Guid DoctorScheduleId { get; set; }

        public Guid ScheduleId { get; set; }

        public Guid DoctorId { get; set; }

        public bool Active { get; set; }

        //navegation properties
        public Schedule Schedule { get; set; }

        public Doctor Doctor { get; set; }

        public AuditLog InactivateDoctorSchedule()
        {
            var log = AuditLog.AddLog("DoctorSchedules", "Acive", "true", "false", DoctorScheduleId, "Update");
            Active = false;
            return log;
        }
    }
}