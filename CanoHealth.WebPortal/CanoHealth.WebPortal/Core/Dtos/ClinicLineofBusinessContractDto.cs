using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ClinicLineofBusinessContractDto
    {
        public Guid Id { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public string Name { get; set; }

        public Guid ContractLineofBusinessId { get; set; }
    }
}