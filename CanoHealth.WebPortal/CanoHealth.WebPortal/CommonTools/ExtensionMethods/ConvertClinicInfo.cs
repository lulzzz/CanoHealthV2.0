using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.CommonTools.ExtensionMethods
{
    public static class ConvertClinicInfo
    {
        public static IEnumerable<ClinicLineofBusinessContractDto> Convert(this IEnumerable<ClinicLineofBusinessContract> source, IEnumerable<PlaceOfServiceExtension> placeOfServices)
        {
            return source.Select(x => new ClinicLineofBusinessContractDto
            {
                Id = x.Id,
                ContractLineofBusinessId = x.ContractLineofBusinessId,
                PlaceOfServiceId = x.PlaceOfServiceId,
                Name = placeOfServices.FirstOrDefault(p => p.PlaceOfServiceId == x.PlaceOfServiceId).Name
            });
        }
    }

    public class PlaceOfServiceExtension
    {
        public Guid PlaceOfServiceId { get; set; }

        public string Name { get; set; }
    }
}