using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    //Class to access DoctorCorporationContractLink entity
    public interface IDoctorLinkedContractRepository : IRepository<DoctorCorporationContractLink>
    {
        DoctorCorporationContractLink FindLinkedContract(Guid doctorCorporationContractLinkId,
                        Guid doctorId,
                        Guid contractLineofBusinessId);

        IEnumerable<DoctorCorporationContractLink> GetDoctorsLinkedToLineOfBusiness(Guid contractLineOfBusinessId);

        IEnumerable<AuditLog> SaveLinkedContracts(IEnumerable<DoctorCorporationContractLink> contracts);

        IEnumerable<ProviderByLocation> GetLocationProvidersOfThisDoctor(Guid? doctorId,
            Guid placeOfServiceId);

        IEnumerable<AuditLog> RemoveLinkedContracts(List<DoctorCorporationContractLink> doctorCorporationContractLinks);

        IEnumerable<DoctorCorporationContractLink> DoctorCorporationContractLinks(Guid insuranceId);
    }
}
