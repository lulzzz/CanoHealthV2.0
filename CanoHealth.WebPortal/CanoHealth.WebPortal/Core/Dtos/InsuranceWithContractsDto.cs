using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class InsuranceWithContractsDto
    {
        public Guid InsuranceId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string Code { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public IEnumerable<ContractDto> Contracts { get; set; }

        public static InsuranceWithContractsDto Wrap(Insurance insurance)
        {
            return new InsuranceWithContractsDto
            {
                InsuranceId = insurance.InsuranceId,
                Name = insurance.Name,
                Active = insurance.Active,
                Code = insurance.Code,
                PhoneNumber = insurance.PhoneNumber,
                Address = insurance.Address,
                Contracts = insurance.Contracts.Select(ContractDto.Wrap)
            };
        }
    }
}