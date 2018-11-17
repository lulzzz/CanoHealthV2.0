using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ContractBusinessLineClinicRepository : Repository<ClinicLineofBusinessContract>,
        IContractBusinessLineClinicRepository
    {
        public ContractBusinessLineClinicRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<AuditLog> GetLogsWhileRemoveItems(IEnumerable<ClinicLineofBusinessContract> clinicLineofBusiness)
        {
            var auditLogs = new List<AuditLog>();
            var collection = clinicLineofBusiness.Select(x => new List<AuditLog>
            {
                AuditLog.AddLog("ClinicLineofBusinessContract", "ContractLineofBusinessId",
                    x.ContractLineofBusinessId.ToString(), null, x.Id, "Delete"),
                AuditLog.AddLog("ClinicLineofBusinessContract", "PlaceOfServiceId", x.PlaceOfServiceId.ToString(), null,
                    x.Id, "Delete"),
            });

            foreach (var item in collection)
            {
                auditLogs.AddRange(item);
            }

            RemoveRange(clinicLineofBusiness);

            return auditLogs;
        }

        public IEnumerable<PlaceOfService> GetLocationsByBusinessLines(Guid contractLineofBusinessId)
        {
            var result = EnumarableGetAll(x => x.ContractLineofBusinessId == contractLineofBusinessId,
                includeProperties: new Expression<Func<ClinicLineofBusinessContract, object>>[]
                {
                    pos => pos.Clinic
                })
                .Select(x => x.Clinic)
                .Where(l => l.Active).ToList();

            return result;
        }

        public IEnumerable<ClinicLineofBusinessContract> GetContractLineofBusinessLocations(Guid insuranceId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetContractLineofBusinessLocationByInsurance] @InsuranceId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId }
                ).ToList();
            return result;
        }

        public IEnumerable<ClinicLineofBusinessContract> GetContractLineofBusinessLocations(Guid insuranceId, Guid lineofBusinessId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetContractLocationsByInsuranceAndLineofbusiness] @InsuranceId, @PlanTypeId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId },
                    new SqlParameter("@PlanTypeId", SqlDbType.UniqueIdentifier) { Value = lineofBusinessId }
                ).ToList();
            return result;
        }

        public IEnumerable<ClinicLineofBusinessContract> GetContractLineofBusinessLocationsByContract(Guid contractId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetContractLocationsByContract] @ContractId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@ContractId", SqlDbType.UniqueIdentifier) { Value = contractId }
                ).ToList();
            return result;
        }

        public IEnumerable<ClinicLineofBusinessContract> ContractLineofBusinessLocations(Guid contractLineofBusinessId)
        {
            var result = EnumarableGetAll(filter: x => x.ContractLineofBusinessId == contractLineofBusinessId &&
            x.Active.HasValue && x.Active.Value).ToList();
            return result;
        }
    }
}
