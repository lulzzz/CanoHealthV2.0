using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IOutofNetworkContractRepository : IRepository<OutOfNetworkContract>
    {
        IEnumerable<AuditLog> SaveContracts(IEnumerable<OutOfNetworkContract> contracts);
        OutOfNetworkContract GetOutOfNetworkContractByDoctorAndInsurnace(Guid doctorId, Guid insuranceId);
        IEnumerable<OutOfNetworkContract> GetActiveOutOfNetworkContractsByDoctor(Guid doctorId);
    }
}
