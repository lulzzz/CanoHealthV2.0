using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class InsuranceDto
    {
        public Guid InsuranceId { get; set; }

        public string Name { get; set; }

        public bool? Active { get; set; }
    }
}