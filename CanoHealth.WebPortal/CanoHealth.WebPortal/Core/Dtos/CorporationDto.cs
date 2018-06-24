using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class CorporationDto
    {
        public Guid CorporationId { get; set; }

        public string CorporationName { get; set; }

        public string Npi { get; set; }

        public string TaxId { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public bool Active { get; set; }
    }
}