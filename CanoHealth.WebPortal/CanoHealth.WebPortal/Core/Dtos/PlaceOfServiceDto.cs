using System;
using System.Collections.Generic;
using System.Linq;
using CanoHealth.WebPortal.Core.Domain;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class PlaceOfServiceDto
    {
        public Guid PlaceOfServiceId { get; set; }

        public Guid CorporationId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public bool Active { get; set; }

        public IEnumerable<LicenseDto> PosLicenses { get; set; }

        public PlaceOfServiceDto()
        {
            PosLicenses = new List<LicenseDto>();
        }

        public static PlaceOfServiceDto Wrap(PlaceOfService placeOfService)
        {
            return new PlaceOfServiceDto
            {
                PlaceOfServiceId = placeOfService.PlaceOfServiceId,
                CorporationId = placeOfService.CorporationId,
                Name = placeOfService.Name,
                Address = placeOfService.Address,
                PhoneNumber = placeOfService.PhoneNumber,
                FaxNumber = placeOfService.FaxNumber,
                Active = placeOfService.Active,
                PosLicenses = placeOfService.PosLicenses.Select(LicenseDto.Wrap)
            };
        }
    }
}