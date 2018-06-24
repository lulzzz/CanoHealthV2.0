using CanoHealth.WebPortal.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class DoctorClinicDto
    {
        public Guid? DoctorClinicId { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public bool ActiveDoctorClinicRelationship { get; set; }

        public DateTime? FromDateTime { get; set; }

        public DateTime? ToDateTime { get; set; }

        //Doctor section
        public Guid? DoctorId { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LAST NAME")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "DOB")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "DEGREE")]
        public string Degree { get; set; }

        [Required]
        [Display(Name = "SSN")]
        [StringLength(11)]
        public string SocialSecurityNumber { get; set; }

        [Required]
        [Display(Name = "NPI")]
        [StringLength(20)]
        public string NpiNumber { get; set; }

        [Required]
        [Display(Name = "CAQH")]
        [StringLength(20)]
        public string CaqhNumber { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool? Active { get; set; }//is the doctor still working in cano

        public Doctor ConvertDoctor()
        {
            return new Doctor
            {
                DoctorId = DoctorId ?? Guid.NewGuid(),
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                Degree = Degree,
                SocialSecurityNumber = SocialSecurityNumber,
                NpiNumber = NpiNumber,
                CaqhNumber = CaqhNumber,
                Active = Active ?? true
            };
        }

        public DoctorClinic ConvertDoctorClinic()
        {
            return new DoctorClinic
            {
                DoctorClinicId = DoctorClinicId ?? Guid.NewGuid(),
                DoctorId = DoctorId.Value,
                PlaceOfServiceId = PlaceOfServiceId,
                Active = ActiveDoctorClinicRelationship,
                FromDateTime = FromDateTime
            };
        }
    }
}