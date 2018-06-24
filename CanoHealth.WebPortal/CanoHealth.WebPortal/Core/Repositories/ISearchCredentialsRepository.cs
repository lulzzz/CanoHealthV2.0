using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.ViewModels;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface ISearchCredentialsRepository : IRepository<SearchCredentialsResultViewModel>
    {
        IEnumerable<SearchResultDoctorInfoDto> GetSearchResultsByInsuranceAndLocation(Guid corporationId, Guid insuranceId, Guid locationId);

        IEnumerable<SearchResultLocationInfoDto> GetSearchResultsByInsuranceAndDoctor(Guid corporationId, Guid insuranceId, Guid doctorId);
    }
}
