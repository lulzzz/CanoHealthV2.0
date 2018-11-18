using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ProviderByLocationRepository : Repository<ProviderByLocation>, IProviderByLocationRepository
    {
        public ProviderByLocationRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<ProviderByLocation> ProviderByLocations(Guid insuranceId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetDoctorProviderByLocation] @InsuranceId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId }
                ).ToList();
            return result;
        }

        public IEnumerable<ProviderByLocation> ProviderByLocationsAndLineofbusiness(Guid insuranceId, Guid lineofBusinessId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetDoctorProviderByLocationAndLineofbusiness] @InsuranceId, @PlanTypeId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId },
                    new SqlParameter("@PlanTypeId", SqlDbType.UniqueIdentifier) { Value = lineofBusinessId }
                ).ToList();
            return result;

        }

        public IEnumerable<ProviderByLocation> ProviderByLocationsAndContract(Guid contractId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetDoctorProviderByLocationAndContract] @ContractId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@ContractId", SqlDbType.UniqueIdentifier) { Value = contractId }
                ).ToList();
            return result;
        }

        public IEnumerable<ProviderByLocation> ProviderByLocationsAndDoctor(Guid doctorId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetDoctorProviderByLocationAndDoctor] @DoctorId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@DoctorId", SqlDbType.UniqueIdentifier) { Value = doctorId }
                ).ToList();
            return result;
        }

        public IEnumerable<ProviderByLocation> GetActiveProvidersByLocation(
            Guid doctorCorporationContractLinkId)
        {
            return EnumarableGetAll(p =>
                p.DoctorCorporationContractLinkId == doctorCorporationContractLinkId &&
                p.Active);
        }

        public IEnumerable<AuditLog> SaveProviderByLocation(IEnumerable<ProviderByLocation> providersByLocations)
        {
            return SaveItems(providersByLocations,
                (collection, item) => collection.Any(p =>
                p.PlaceOfServiceId == item.PlaceOfServiceId &&
                p.DoctorCorporationContractLinkId == item.DoctorCorporationContractLinkId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<ProviderByLocation> providerByLocations,
            Func<DbSet<ProviderByLocation>, ProviderByLocation, bool> existProvider)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var provider in providerByLocations)
            {
                if (existProvider(Entities, provider))
                {
                    //buscalo y modificalo
                    var providerStoredInDb = SingleOrDefault(x =>
                        x.DoctorCorporationContractLinkId == provider.DoctorCorporationContractLinkId &&
                        x.PlaceOfServiceId == provider.PlaceOfServiceId);
                    var logs = providerStoredInDb.Modify(provider);
                    auditLogs.AddRange(logs);
                }
                else
                {
                    //Crealo
                    Add(provider);

                    auditLogs.AddRange(new List<AuditLog>
                    {

                        AuditLog.AddLog("ProviderByLocations",
                            "DoctorCorporationContractLinkId",
                            null,
                            provider.DoctorCorporationContractLinkId.ToString(),
                            provider.ProviderByLocationId,
                           "Insert"),
                        AuditLog.AddLog("ProviderByLocations",
                            "PlaceOfServiceId",
                            null,
                            provider.PlaceOfServiceId.ToString(),
                            provider.ProviderByLocationId,
                            "Insert"),
                         AuditLog.AddLog("ProviderByLocations",
                            "ProviderEffectiveDate",
                            null,
                            provider.ProviderEffectiveDate.ToString(),
                            provider.ProviderByLocationId,
                            "Update"),
                         AuditLog.AddLog("ProviderByLocations",
                            "LocacionProviderNumber",
                            null,
                            provider.LocacionProviderNumber,
                             provider.ProviderByLocationId,
                            "Update"),
                         AuditLog.AddLog("ProviderByLocations",
                            "Active",
                            null,
                            provider.Active.ToString(),
                            provider.ProviderByLocationId,
                            "Update")
                    });

                }
            }
            return auditLogs;
        }

        public IEnumerable<AuditLog> InactivateProviders(IEnumerable<ProviderByLocation> locationProviders)
        {
            return locationProviders.Select(provider => provider.Inactivate()).ToList();
        }

        public IEnumerable<AuditLog> ActivateProviders(IEnumerable<ProviderByLocation> locationProviders)
        {
            return locationProviders.Select(provider => provider.Activate()).ToList();
        }

        public IEnumerable<AuditLog> RemoveProviders(IEnumerable<ProviderByLocation> locationProviders)
        {
            var audtiLogs = new List<AuditLog>();
            foreach (var provider in locationProviders)
            {
                audtiLogs.AddRange(
                    new List<AuditLog>
                    {
                        AuditLog.AddLog("ProviderByLocations",
                        "DoctorCorporationContractLinkId",
                        provider.DoctorCorporationContractLinkId.ToString(),
                        null,
                        provider.ProviderByLocationId,
                        "Delete"),

                        AuditLog.AddLog("ProviderByLocations",
                        "PlaceOfServiceId",
                        provider.PlaceOfServiceId.ToString(),
                        null,
                        provider.ProviderByLocationId,
                        "Delete"),

                        AuditLog.AddLog("ProviderByLocations",
                        "ProviderEffectiveDate",
                        provider.ProviderEffectiveDate.ToString(),
                        null,
                        provider.ProviderByLocationId,
                        "Delete"),

                        AuditLog.AddLog("ProviderByLocations",
                        "LocacionProviderNumber",
                        provider.LocacionProviderNumber,
                        null,
                        provider.ProviderByLocationId,
                        "Delete"),

                        AuditLog.AddLog("ProviderByLocations",
                        "Active",
                        provider.Active.ToString(),
                        null,
                        provider.ProviderByLocationId,
                        "Delete")
                    });
                Remove(provider);
            }
            return audtiLogs;
        }
    }
}