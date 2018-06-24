using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Dtos.Npi
{
    public class ResultDto
    {
        public IEnumerable<AddressDto> addresses { get; set; }

        public BasicDto basic { get; set; }

        public long created_epoch { get; set; }

        public string enumeration_type { get; set; }

        public long last_updated_epoch { get; set; }

        public long number { get; set; }

        public IEnumerable<OtherNamesDto> other_names { get; set; }

        public IEnumerable<TaxonomiesDto> taxonomies { get; set; }

        public ResultDto()
        {
            addresses = new List<AddressDto>();

            other_names = new List<OtherNamesDto>();

            taxonomies = new List<TaxonomiesDto>();
        }
    }
}