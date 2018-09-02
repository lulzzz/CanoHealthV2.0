using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Doctor
    {
        public Guid DoctorId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(255)]
        public string Degree { get; set; }

        [Required]
        [StringLength(11)]
        public string SocialSecurityNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string NpiNumber { get; set; }

        [StringLength(20)]
        public string CaqhNumber { get; set; }

        public bool Active { get; set; }

        public ICollection<DoctorClinic> Clinics { get; set; }

        public ICollection<DoctorSchedule> Schedules { get; set; }

        public Doctor()
        {
            Clinics = new Collection<DoctorClinic>();
            Schedules = new Collection<DoctorSchedule>();
        }

        public AuditLog Inactivate()
        {
            var auditLog = AuditLog.AddLog("Doctor", "Active", true.ToString(), false.ToString(), DoctorId, "Update");
            Active = false;
            return auditLog;
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public bool AreTheSamePerson(Doctor doctor)
        {
            return FirstName.Equals(doctor.FirstName, StringComparison.CurrentCultureIgnoreCase) &&
                   LastName.Equals(doctor.LastName, StringComparison.CurrentCultureIgnoreCase) &&
                   DateOfBirth == doctor.DateOfBirth &&
                   SocialSecurityNumber == doctor.SocialSecurityNumber &&
                   NpiNumber.Equals(doctor.NpiNumber, StringComparison.CurrentCultureIgnoreCase) &&
                   CaqhNumber.Equals(doctor.CaqhNumber, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}