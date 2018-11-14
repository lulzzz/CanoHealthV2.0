using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IContractBusinessLineClinicRepository : IRepository<ClinicLineofBusinessContract>
    {
        IEnumerable<AuditLog> GetLogsWhileRemoveItems(IEnumerable<ClinicLineofBusinessContract> clinicLineofBusiness);
        IEnumerable<PlaceOfService> GetLocationsByBusinessLines(Guid contractLineofBusinessId);
        IEnumerable<ClinicLineofBusinessContract> GetContractLineofBusinessLocations(Guid insuranceId);
        IEnumerable<ClinicLineofBusinessContract> ContractLineofBusinessLocations(Guid contractLineofBusinessId);
    }
}