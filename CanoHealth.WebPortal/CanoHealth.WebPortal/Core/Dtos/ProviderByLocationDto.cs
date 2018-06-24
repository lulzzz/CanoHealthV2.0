using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ProviderByLocationDto
    {
        public Guid ProviderByLocationId { get; set; } //PK

        //FK from DoctorCorporationContractLink
        public Guid DoctorCorporationContractLinkId { get; set; }

        //It's going to be the glue with Locations. It is not a FK
        public Guid PlaceOfServiceId { get; set; }

        public string PlaceOfServiceName { get; set; }

        public DateTime? ProviderEffectiveDate { get; set; }

        public string LocacionProviderNumber { get; set; }

        public bool Active { get; set; }
    }
}