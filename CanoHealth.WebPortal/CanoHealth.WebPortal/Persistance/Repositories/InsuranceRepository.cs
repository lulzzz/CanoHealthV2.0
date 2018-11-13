using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using CanoHealth.WebPortal.Core.Specifications.Insurances;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class InsuranceRepository : Repository<Insurance>, IInsuranceRepository
    {
        public InsuranceRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Insurance> GetActiveInsurances()
        {
            return EnumarableGetAll(i => i.Active).ToList();
        }

        public IEnumerable<Insurance> GetInsurancesByNames(IEnumerable<string> names)
        {
            var insurances = new List<Insurance>();
            foreach (var item in names)
            {
                var insurance = SingleOrDefault(i => i.Name.Equals(item, StringComparison.InvariantCulture));
                if (insurance == null)
                {

                }
                else
                {
                    insurances.Add(insurance);
                }
            }

            return insurances;
        }

        public Insurance GetByName(string name)
        {
            return EnumarableGetAll(includeProperties: new Expression<Func<Insurance, object>>[] { c => c.Contracts })
                .SingleOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public Insurance GetInsuranceById(Guid insuranceId)
        {
            return Get(insuranceId);
        }

        public Insurance GetWithContracts(Guid insurancesId)
        {
            return EnumarableGetAll(filter: i => i.Active, 
                                    includeProperties: new Expression<Func<Insurance, object>>[] { c => c.Contracts })
                .SingleOrDefault(i => i.InsuranceId == insurancesId);
        }

        public Insurance GetOtherInsuranceWithSameName(string insuranceName, Guid insuranceId)
        {
            return SingleOrDefault(
                i => i.Name.Equals(insuranceName, StringComparison.InvariantCultureIgnoreCase) &&
                     i.InsuranceId != insuranceId);
        }

        public IEnumerable<AuditLog> SaveItems(IEnumerable<Insurance> insurances)
        {
            return SaveInsurances(insurances, (set, insurance) => set.Any(i => i.InsuranceId == insurance.InsuranceId));
        }

        private IEnumerable<AuditLog> SaveInsurances(IEnumerable<Insurance> insurances, Func<DbSet<Insurance>, Insurance, bool> expression)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var insurance in insurances)
            {
                if (expression(Entities, insurance))
                {
                    var insuranceStoredInDb = QueryableGetAll(includeProperties: new Expression<Func<Insurance, object>>[] { c => c.Contracts })
                        .SingleOrDefault(i => i.InsuranceId == insurance.InsuranceId);
                    if (insuranceStoredInDb != null)
                    {
                        var logs = insuranceStoredInDb.ModifyInsurance(insurance);
                        auditLogs.AddRange(logs);
                    }
                }
                else
                {
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog("Insurance", "Code", null, insurance.Code, insurance.InsuranceId, "Create"),
                        AuditLog.AddLog("Insurance", "Name", null, insurance.Name, insurance.InsuranceId, "Create"),
                        AuditLog.AddLog("Insurance", "PhoneNumber", null, insurance.PhoneNumber, insurance.InsuranceId, "Create"),
                        AuditLog.AddLog("Insurance", "Address", null, insurance.Address, insurance.InsuranceId, "Create"),
                        AuditLog.AddLog("Insurance", "Active", null, insurance.Active.ToString(), insurance.InsuranceId, "Create")
                    });
                    Add(insurance);
                }
            }
            return auditLogs;
        }

        public async Task<Insurance> GetWithContractsAsync(Guid insuranceId)
        {
            var specification = new InsuranceContractSpecification(insuranceId);
            return await SingleOrDefaultAsync(specification);
        }
    }
}