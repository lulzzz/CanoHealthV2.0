using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class PlaceOfServiceRepository : Repository<PlaceOfService>, IPlaceOfServiceRepository
    {
        public PlaceOfServiceRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<PlaceOfService> GetActivePlaceOfServices()
        {
            return EnumarableGetAll(p => p.Active);
        }

        public PlaceOfService GetPlaceOfServiceByName(string name)
        {
            return SingleOrDefault(pos => pos.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public PlaceOfService FindOtherPlaceOfServiceWithSameName(string name, Guid placeOfServiceId)
        {
            var placeOfService = SingleOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                                     p.PlaceOfServiceId != placeOfServiceId);
            return placeOfService;
        }

        public PlaceOfService GetPlaceOfService(Guid placeOfServiceId)
        {
            return EnumarableGetAll(filter: p => p.Active,
                 includeProperties: new Expression<Func<PlaceOfService, object>>[] { l => l.PosLicenses.Select(t => t.LicenseType) })
                 .SingleOrDefault(p => p.PlaceOfServiceId == placeOfServiceId);
        }

        public PlaceOfService GetLocationWithContractLineOfBusines(Guid placeOfServiceId)
        {
            return QueryableGetAll(l => l.Active,
                includeProperties: new Expression<Func<PlaceOfService, object>>[]
                {
                    clb => clb.ClinicLineofBusiness
                })
                .SingleOrDefault(l => l.PlaceOfServiceId == placeOfServiceId);
        }

        public IEnumerable<PlaceOfService> GetAllActivePlaceOfServices(Guid? corporationId)
        {
            var result = EnumarableGetAll(p => p.Active).ToList();
            if (corporationId != null)
            {
                result = result.Where(c => c.CorporationId == corporationId).ToList();
            }
            return result;
        }
    }
}