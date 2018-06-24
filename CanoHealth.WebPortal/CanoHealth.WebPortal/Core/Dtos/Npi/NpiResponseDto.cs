using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Dtos.Npi
{
    public class NpiResponseDto
    {
        public int result_count { get; set; }

        public IEnumerable<ResultDto> results { get; set; }

        public NpiResponseDto()
        {
            results = new List<ResultDto>();
        }
    }
}