using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class SearchResultDoctorInfoDto
    {
        public string Degree { get; set; }

        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string NpiNumber { get; set; }

        public string CaqhNumber { get; set; }

        public string ProviderNumberByInsurance { get; set; }

        public List<DoctorLinkedToLineOfBusinessDto> LineOfBusiness { get; set; }

        public SearchResultDoctorInfoDto()
        {
            LineOfBusiness = new List<DoctorLinkedToLineOfBusinessDto>();
        }
    }
}