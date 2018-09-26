using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class DoctorDto
    {
        public Guid DoctorId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Degree { get; set; }

        public string SocialSecurityNumber { get; set; }

        public string NpiNumber { get; set; }

        public string CaqhNumber { get; set; }

        public bool Active { get; set; }

        public static DoctorDto Wrap(Doctor doctor)
        {
            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                FullName = $"{doctor.FirstName} {doctor.LastName}",
                DateOfBirth = doctor.DateOfBirth,
                Degree = doctor.Degree,
                //SocialSecurityNumber = doctor.SocialSecurityNumber,
                NpiNumber = doctor.NpiNumber,
                CaqhNumber = doctor.CaqhNumber,
                Active = doctor.Active,
            };
        }
    }
}