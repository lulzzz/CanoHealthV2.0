using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IProviderByLocationRepository : IRepository<ProviderByLocation>
    {
        IEnumerable<AuditLog> SaveProviderByLocation(IEnumerable<ProviderByLocation> providersByLocations);

        IEnumerable<ProviderByLocation> GetActiveProvidersByLocation(
            Guid doctorCorporationContractLinkId);
        IEnumerable<ProviderByLocation> ProviderByLocations(Guid insuranceId);
        IEnumerable<ProviderByLocation> ProviderByLocationsAndLineofbusiness(Guid insuranceId, Guid lineofBusinessId);
        IEnumerable<AuditLog> InactivateProviders(IEnumerable<ProviderByLocation> locationProviders);
        IEnumerable<AuditLog> ActivateProviders(IEnumerable<ProviderByLocation> locationProviders);
        IEnumerable<AuditLog> RemoveProviders(IEnumerable<ProviderByLocation> locationProviders);
    }
}