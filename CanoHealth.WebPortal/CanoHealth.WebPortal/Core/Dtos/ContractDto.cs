using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ContractDto
    {
        public Guid ContractId { get; set; }

        public string GroupNumber { get; set; }

        public Guid CorporationId { get; set; }

        public Guid InsuranceId { get; set; }

        public bool Active { get; set; }

        public static ContractDto Wrap(Contract contract)
        {
            return new ContractDto
            {
                ContractId = contract.ContractId,
                CorporationId = contract.CorporationId,
                InsuranceId = contract.InsuranceId,
                GroupNumber = contract.GroupNumber,
                Active = contract.Active
            };
        }

        public Contract Convert()
        {
            return new Contract
            {
                ContractId = ContractId,
                InsuranceId = InsuranceId,
                CorporationId = CorporationId,
                GroupNumber = GroupNumber,
                Active = Active
            };
        }
    }
}