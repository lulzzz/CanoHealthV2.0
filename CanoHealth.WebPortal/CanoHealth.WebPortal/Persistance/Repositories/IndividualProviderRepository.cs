using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class IndividualProviderRepository : Repository<DoctorIndividualProvider>, IIndividualProviderRepository
    {
        public IndividualProviderRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<DoctorIndividualProvider> GetIndividualProviders(Guid? doctorId)
        {
            var individualProviders = EnumarableGetAll(filter: dip => dip.Active.HasValue && dip.Active.Value,
                includeProperties: new Expression<Func<DoctorIndividualProvider, object>>[]
                {d => d.Doctor, i => i.Insurance});

            if (doctorId != null)
            {
                individualProviders = individualProviders.Where(ip => ip.DoctorId == doctorId);
            }

            return individualProviders;
        }

        public IEnumerable<DoctorIndividualProvider> GetIndividualProvidersByInsurance(Guid insuranceId)
        {
            var individualProviders = EnumarableGetAll(filter: dip => dip.InsuranceId == insuranceId &&
                                                               dip.Active.HasValue && dip.Active.Value,
                includeProperties: new Expression<Func<DoctorIndividualProvider, object>>[]
                {
                    d => d.Doctor, i => i.Insurance
                });

            return individualProviders;
        }

        public DoctorIndividualProvider ExistIndividualProvider(Guid doctorId, Guid insuranceId)
        {
            return SingleOrDefault(ip => ip.DoctorId == doctorId &&
                                         ip.InsuranceId == insuranceId &&
                                         ip.Active.HasValue &&
                                         ip.Active.Value);
        }

        public DoctorIndividualProvider GetIndividualProviderByProviderNumber(Guid doctorIndividualProviderId, string individualProviderProviderNumber)
        {
            return SingleOrDefault(ip => ip.DoctorIndividualProviderId != doctorIndividualProviderId &&
                                   ip.ProviderNumber.Equals(individualProviderProviderNumber,StringComparison.InvariantCultureIgnoreCase) &&
                                   ip.Active.HasValue &&
                                   ip.Active.Value);
        }

        public DoctorIndividualProvider GetIndividualProviderByDoctorAndInsurance(DoctorIndividualProvider doctorIndividualProvider)
        {
            return SingleOrDefault(ip =>
                ip.DoctorIndividualProviderId != doctorIndividualProvider.DoctorIndividualProviderId &&
                ip.DoctorId == doctorIndividualProvider.DoctorId &&
                ip.InsuranceId == doctorIndividualProvider.InsuranceId &&
                ip.Active.HasValue &&
                ip.Active.Value);
        }

        public IEnumerable<AuditLog> SaveIndividualProviders(List<DoctorIndividualProvider> doctorIndividualProviders)
        {
            return SaveItems(doctorIndividualProviders,
                 (collection, item) =>
                     collection.Any(x => x.DoctorIndividualProviderId == item.DoctorIndividualProviderId));
        }

        private IEnumerable<AuditLog> SaveItems(List<DoctorIndividualProvider> doctorIndividualProviders,
            Func<DbSet<DoctorIndividualProvider>, DoctorIndividualProvider, bool> existIndividualProvider)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var doctorIndividualProvider in doctorIndividualProviders)
            {
                if (existIndividualProvider(Entities, doctorIndividualProvider))
                {
                    var individualProviderStoredInDb = Get(doctorIndividualProvider.DoctorIndividualProviderId);
                    var modifyLogs = individualProviderStoredInDb.Modify(doctorIndividualProvider);
                    auditLogs.AddRange(modifyLogs);
                }
                else
                {
                    Add(doctorIndividualProvider);
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog("DoctorIndividualProviders",
                            "InsuranceId",
                            null,
                            doctorIndividualProvider.InsuranceId.ToString(),
                            doctorIndividualProvider.DoctorIndividualProviderId,
                            "Insert"),
                        AuditLog.AddLog("DoctorIndividualProviders",
                            "DoctorId",
                            null,
                            doctorIndividualProvider.DoctorId.ToString(),
                            doctorIndividualProvider.DoctorIndividualProviderId,
                            "Insert"),
                        AuditLog.AddLog("DoctorIndividualProviders",
                            "ProviderNumber",
                            null,
                            doctorIndividualProvider.ProviderNumber,
                            doctorIndividualProvider.DoctorIndividualProviderId,
                            "Insert"),
                        AuditLog.AddLog("DoctorIndividualProviders",
                            "IndividualProviderEffectiveDate",
                            null,
                            doctorIndividualProvider.IndividualProviderEffectiveDate.ToString(),
                            doctorIndividualProvider.DoctorIndividualProviderId,
                            "Insert"),
                    });
                }
            }

            return auditLogs;
        }
    }
}