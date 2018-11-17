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
    public class ContractBusinessLineRepository : Repository<ContractLineofBusiness>, IContractBusinessLineRepository
    {
        public ContractBusinessLineRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<ContractLineofBusiness> GetContractBusinessLinesWithClinics(string contractId)
        {
            var iqueryableresult = QueryableGetAll(filter: clb => clb.Active.HasValue && clb.Active.Value, includeProperties: new Expression<Func<ContractLineofBusiness, object>>[]
                {
                    clb => clb.ClinicLineofBusiness.Select(pos => pos.Clinic),
                    bl => bl.LineOfBusiness
                });
            if (!String.IsNullOrEmpty(contractId))
            {
                var guidcontractId = Guid.Parse(contractId);
                iqueryableresult = iqueryableresult.Where(x => x.ContractId == guidcontractId);
            }
            return iqueryableresult.ToList();
        }

        public IEnumerable<ContractLineofBusiness> GetContractBusinessLines(Guid contractId)
        {
            var businessLinesOfTheContract = QueryableGetAll(
                filter: x => x.ContractId == contractId && x.Active.HasValue && x.Active.Value,
                includeProperties: new Expression<Func<ContractLineofBusiness, object>>[]
                {
                    pt => pt.LineOfBusiness
                }).ToList();
            return businessLinesOfTheContract;
        }

        public IEnumerable<ContractLineofBusiness> ContractLineofBusinesses(Guid insuranceId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetContractLineofBusinessByInsurance] @InsuranceId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId }
                ).ToList();
            return result;
        }

        public IEnumerable<ContractLineofBusiness> ContractLineofBusinesses(Guid insuranceId, Guid planTypeId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetContractLineofBusinessByInsuranceAndLineofBusiness] @InsuranceId,@PlanTypeId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId },
                    new SqlParameter("@PlanTypeId", SqlDbType.UniqueIdentifier) { Value = planTypeId }
                ).ToList();
            return result;
        }

        public ContractLineofBusiness GetContractLineofBusinessAndLocations(Guid contractLineofBusinessId)
        {
            var contracBusinessLine = QueryableGetAll(includeProperties:
                new Expression<Func<ContractLineofBusiness, object>>[]
                {
                    clb => clb.ClinicLineofBusiness.Select(pos => pos.Clinic),
                    bl => bl.LineOfBusiness
                }).SingleOrDefault(cbl => cbl.ContractLineofBusinessId == contractLineofBusinessId);
            return contracBusinessLine;
        }

        public IEnumerable<AuditLog> GetLogsWhileRemoveItems(List<ContractLineofBusiness> contractLineofBusinesses)
        {
            var auditLogs = new List<AuditLog>();

            var collection = contractLineofBusinesses.Select(x => new List<AuditLog>
            {
                AuditLog.AddLog("ContractLineofBusiness", "ContractId", x.ContractId.ToString(), null, x.ContractLineofBusinessId, "Delete"),
                AuditLog.AddLog("ContractLineofBusiness", "PlanTypeId", x.PlanTypeId.ToString(), null, x.ContractLineofBusinessId, "Delete"),
            });

            foreach (var item in collection)
            {
                auditLogs.AddRange(item);
            }

            RemoveRange(contractLineofBusinesses);

            return auditLogs;
        }

        public ContractLineofBusiness ExistItem(Guid contractId, Guid planTypeId, Guid? contractLineofBusinessId = null)
        {
            if (contractLineofBusinessId != null)
            {
                return SingleOrDefault(x => x.ContractId == contractId &&
                                            x.PlanTypeId == planTypeId &&
                                            x.ContractLineofBusinessId != contractLineofBusinessId &&
                                            x.Active.HasValue && x.Active.Value);
            }

            return SingleOrDefault(x => x.ContractId == contractId &&
                                        x.PlanTypeId == planTypeId &&
                                        x.Active.HasValue && x.Active.Value);
        }
    }
}