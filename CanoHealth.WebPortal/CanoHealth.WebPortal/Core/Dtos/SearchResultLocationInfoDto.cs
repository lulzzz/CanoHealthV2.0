using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class SearchResultLocationInfoDto
    {
        public Guid? PlaceOfServiceId { get; set; }

        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string Address { get; set; }

        public string ProviderNumberByInsurance { get; set; }

        public List<DoctorLinkedToLineOfBusinessDto> LineOfBusiness { get; set; }

        public SearchResultLocationInfoDto()
        {
            LineOfBusiness = new List<DoctorLinkedToLineOfBusinessDto>();
        }
    }
}