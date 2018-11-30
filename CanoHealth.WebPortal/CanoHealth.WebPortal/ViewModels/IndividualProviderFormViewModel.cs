using CanoHealth.WebPortal.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class IndividualProviderFormViewModel
    {
        public Guid DoctorIndividualProviderId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid InsuranceId { get; set; }

        public string InsuranceName { get; set; }

        [Required]
        public string ProviderNumber { get; set; }

        public DateTime IndividualProviderEffectiveDate { get; set; }

        public static IndividualProviderFormViewModel Wrap(DoctorIndividualProvider individualProvider)
        {
            return new IndividualProviderFormViewModel
            {
                DoctorIndividualProviderId = individualProvider.DoctorIndividualProviderId,
                DoctorId = individualProvider.DoctorId,
                InsuranceId = individualProvider.InsuranceId,
                InsuranceName = individualProvider.Insurance.Name,
                ProviderNumber = individualProvider.ProviderNumber,
                IndividualProviderEffectiveDate = individualProvider.IndividualProviderEffectiveDate
            };
        }

        public DoctorIndividualProvider Convert()
        {
            return new DoctorIndividualProvider
            {
                DoctorIndividualProviderId = DoctorIndividualProviderId == Guid.Empty ? Guid.NewGuid() : DoctorIndividualProviderId,
                InsuranceId = InsuranceId,
                DoctorId = DoctorId,
                ProviderNumber = ProviderNumber,
                IndividualProviderEffectiveDate = IndividualProviderEffectiveDate,
                Active = true
            };
        }
    }
}