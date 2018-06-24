using CanoHealth.WebPortal.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class LinkedContractFormDto
    {
        public Guid DoctorCorporationContractLinkId { get; set; }

        public Guid ContractLineofBusinessId { get; set; }

        public Guid DoctorId { get; set; }

        public string Note { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Required]
        public string CorporationName { get; set; }

        [Required]
        public string InsuranceName { get; set; }

        [Required]
        public string GroupNumber { get; set; }

        public DoctorCorporationContractLink Convert()
        {
            return new DoctorCorporationContractLink
            {
                DoctorCorporationContractLinkId = DoctorCorporationContractLinkId == Guid.Empty ?
                    Guid.NewGuid() : DoctorCorporationContractLinkId,
                DoctorId = DoctorId,
                ContractLineofBusinessId = ContractLineofBusinessId,
                EffectiveDate = EffectiveDate,
                Note = Note
            };
        }
    }
}