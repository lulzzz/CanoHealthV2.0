using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorClinic
    {
        public Guid DoctorClinicId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public bool Active { get; set; }

        public DateTime? FromDateTime { get; set; }

        public DateTime? ToDateTime { get; set; }

        //Navegation Properties
        public Doctor Doctor { get; set; }

        public PlaceOfService Clinic { get; set; }

        public IEnumerable<AuditLog> AssignNewDoctorToClinicLogs()
        {
            var auditLogs = new List<AuditLog>();
            auditLogs.AddRange(new List<AuditLog>
            {
                 AuditLog.AddLog("DoctorClinics", "DoctorId", null, DoctorId.ToString(), DoctorClinicId, "Insert"),
                 AuditLog.AddLog("DoctorClinics", "PlaceOfServiceId", null, PlaceOfServiceId.ToString(), DoctorClinicId, "Insert"),
            });
            return auditLogs;
        }

        public AuditLog ReleaseDoctorFromClinic()
        {
            var log = AuditLog.AddLog("DoctorClinics", "Active", true.ToString(), false.ToString(), DoctorClinicId, "Update");
            Active = false;
            return log;
        }
    }
}