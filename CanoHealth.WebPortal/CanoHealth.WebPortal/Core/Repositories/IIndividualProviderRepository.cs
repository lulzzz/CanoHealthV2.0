using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IIndividualProviderRepository : IRepository<DoctorIndividualProvider>
    {
        IEnumerable<DoctorIndividualProvider> GetIndividualProviders(Guid? doctorId);
        IEnumerable<AuditLog> SaveIndividualProviders(List<DoctorIndividualProvider> doctorIndividualProviders);
        DoctorIndividualProvider GetIndividualProviderByProviderNumber(Guid doctorIndividualProviderId, string individualProviderProviderNumber);
        DoctorIndividualProvider GetIndividualProviderByDoctorAndInsurance(DoctorIndividualProvider doctorIndividualProvider);
        DoctorIndividualProvider ExistIndividualProvider(Guid doctorId, Guid insuranceId);
    }
}