using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ContractExtDto
    {
        public Guid ContractId { get; set; }

        public string GroupNumber { get; set; }

        public Guid CorporationId { get; set; }

        public string CorporationName { get; set; }

        public Guid InsuranceId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public static ContractExtDto Wrap(Contract contract)
        {
            return new ContractExtDto
            {
                ContractId = contract.ContractId,
                GroupNumber = contract.GroupNumber,
                Active = contract.Active,
                CorporationId = contract.CorporationId,
                CorporationName = contract.Corporation.CorporationName,
                InsuranceId = contract.InsuranceId,
                Name = contract.Insurance.Name
            };
        }
    }
}