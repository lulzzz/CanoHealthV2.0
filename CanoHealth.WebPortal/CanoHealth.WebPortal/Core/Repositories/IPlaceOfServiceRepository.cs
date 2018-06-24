using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IPlaceOfServiceRepository : IRepository<PlaceOfService>
    {
        IEnumerable<PlaceOfService> GetActivePlaceOfServices();
        PlaceOfService GetPlaceOfServiceByName(string name);
        PlaceOfService FindOtherPlaceOfServiceWithSameName(string name, Guid placeOfServiceId);
        PlaceOfService GetPlaceOfService(Guid placeOfServiceId);
        PlaceOfService GetLocationWithContractLineOfBusines(Guid placeOfServiceId);
        IEnumerable<PlaceOfService> GetAllActivePlaceOfServices(Guid? corporationId);
    }
}
