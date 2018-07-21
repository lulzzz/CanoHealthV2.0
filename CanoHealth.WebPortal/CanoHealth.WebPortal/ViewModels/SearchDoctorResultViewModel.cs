using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.ViewModels
{
    public class SearchDoctorResultViewModel
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

        /*This property is setup just when we know an insurance*/
        public string IndividualProviderNumber { get; set; }

        public string IndividualProviderByLocation { get; set; }

        public static SearchDoctorResultViewModel Wrap(Doctor doctor)
        {
            return new SearchDoctorResultViewModel
            {
                DoctorId = doctor.DoctorId,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                FullName = $"{doctor.FirstName} {doctor.LastName}",
                DateOfBirth = doctor.DateOfBirth,
                Degree = doctor.Degree,
                SocialSecurityNumber = doctor.SocialSecurityNumber,
                NpiNumber = doctor.NpiNumber,
                CaqhNumber = doctor.CaqhNumber,
                Active = doctor.Active,
            };
        }
    }
}