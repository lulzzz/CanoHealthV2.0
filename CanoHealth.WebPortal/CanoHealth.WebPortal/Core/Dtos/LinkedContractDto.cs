using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class LinkedContractDto
    {
        public Guid DoctorCorporationContractLinkId { get; set; }

        public Guid DoctorId { get; set; }

        public string CorporationName { get; set; }

        public string InsuranceName { get; set; }

        public Guid ContractId { get; set; }

        public string GroupNumber { get; set; }

        public List<ContractBusinessLineDto> LineofBusiness { get; set; }

        public LinkedContractDto()
        {
            LineofBusiness = new List<ContractBusinessLineDto>();
        }

        public List<DoctorCorporationContractLink> Convert()
        {
            var linkedContracts = new List<DoctorCorporationContractLink>();
            foreach (var lineofbusiness in LineofBusiness)
            {
                linkedContracts.Add(new DoctorCorporationContractLink
                {
                    DoctorCorporationContractLinkId = DoctorCorporationContractLinkId == Guid.Empty ? Guid.NewGuid() : DoctorCorporationContractLinkId,
                    DoctorId = DoctorId,
                    ContractLineofBusinessId = lineofbusiness.ContractLineofBusinessId
                });
            }
            return linkedContracts;
        }
    }
}