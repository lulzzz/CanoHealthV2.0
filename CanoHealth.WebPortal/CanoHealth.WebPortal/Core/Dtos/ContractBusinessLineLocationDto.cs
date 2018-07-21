using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ContractBusinessLineLocationDto
    {
        public Guid ContractLineofBusinessId { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public Guid CorporationId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public bool Active { get; set; }

        /*This property is setup just when the insurance is passed*/
        public Guid? InsuranceId { get; set; }
    }
}