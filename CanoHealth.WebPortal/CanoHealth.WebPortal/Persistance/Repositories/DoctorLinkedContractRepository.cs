using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class DoctorLinkedContractRepository : Repository<DoctorCorporationContractLink>, IDoctorLinkedContractRepository
    {
        public DoctorLinkedContractRepository(ApplicationDbContext context) : base(context) { }

        public DoctorCorporationContractLink FindLinkedContract(
            Guid doctorCorporationContractLinkId,
            Guid doctorId,
            Guid contractLineofBusinessId)
        {
            return FirstOrDefault(x => x.DoctorId == doctorId &&
                                  x.ContractLineofBusinessId == contractLineofBusinessId &&
                                  x.DoctorCorporationContractLinkId != doctorCorporationContractLinkId);
        }

        /*Get the list of all active doctors who are linked to specific contract and line of business*/
        public IEnumerable<DoctorCorporationContractLink> GetDoctorsLinkedToLineOfBusiness(Guid contractLineOfBusinessId)
        {
            var result = EnumarableGetAll(dccl => dccl.ContractLineofBusinessId == contractLineOfBusinessId &&
            dccl.Active.HasValue && dccl.Active.Value,
                 includeProperties: new Expression<Func<DoctorCorporationContractLink, object>>[]
                 {
                     d => d.Doctor,
                     ipbl => ipbl.ProvidersByLocations //Individual provider by location
                 })
                 .ToList();
            return result;
        }

        public IEnumerable<ProviderByLocation> GetLocationProvidersOfThisDoctor(Guid? doctorId,
            Guid placeOfServiceId)
        {
            var providersByLocation = new List<ProviderByLocation>();
            var results = EnumarableGetAll(x => x.DoctorId == doctorId,
                includeProperties: new Expression<Func<DoctorCorporationContractLink, object>>[]
                {
                    pbl => pbl.ProvidersByLocations
                })
                .ToList();
            foreach (var item in results)
            {
                providersByLocation.AddRange(item.ProvidersByLocations
                    .Where(l => l.PlaceOfServiceId == placeOfServiceId));
            }
            return providersByLocation;
        }

        public IEnumerable<DoctorCorporationContractLink> DoctorCorporationContractLinks(Guid insuranceId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetDoctorCorporationContractLinksByInsurance] @InsuranceId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId }
                ).ToList();
            return result;
        }

        public IEnumerable<DoctorCorporationContractLink> DoctorCorporationContractLinksByContract(Guid contractId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetDoctorCorporationContractLinksByContract] @ContractId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@ContractId", SqlDbType.UniqueIdentifier) { Value = contractId }
                ).ToList();
            return result;
        }

        public IEnumerable<DoctorCorporationContractLink> DoctorCorporationContractLinksByLineofbusiness(Guid insuranceId, Guid lineofBusinessId)
        {
            //parametrized queries instead string concatenations protect you against SQL Injection
            var query = "EXEC [dbo].[GetContractCorporationDoctorByInsuranceAndLineofBusiness] @InsuranceId,@PlanTypeId";
            var result = GetWithRawSql(query,
                    new SqlParameter("@InsuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId },
                    new SqlParameter("@PlanTypeId", SqlDbType.UniqueIdentifier) { Value = lineofBusinessId }
                ).ToList();
            return result;
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<DoctorCorporationContractLink> contracts,
            Func<DbSet<DoctorCorporationContractLink>, DoctorCorporationContractLink, bool> existLinkedContract)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var contract in contracts)
            {
                if (existLinkedContract(Entities, contract))
                {
                    var linkedContractStoredInDb = Get(contract.DoctorCorporationContractLinkId);
                    var logs = linkedContractStoredInDb.Modify(contract.ContractLineofBusinessId,
                        contract.DoctorId, contract.EffectiveDate, contract.Note);
                    auditLogs.AddRange(logs);
                }
                else
                {
                    Add(contract);
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog(
                            "DoctorCorporationContractLinks",
                            "ContractLineofBusinessId",
                            null,
                            contract.ContractLineofBusinessId.ToString(),
                            contract.DoctorCorporationContractLinkId,
                            "Insert"
                            ),
                        AuditLog.AddLog(
                            "DoctorCorporationContractLinks",
                            "DoctorId",
                            null,
                            contract.DoctorId.ToString(),
                            contract.DoctorCorporationContractLinkId,
                            "Insert"
                            ),
                        AuditLog.AddLog(
                            "DoctorCorporationContractLinks",
                            "Note",
                            null,
                            contract.Note,
                            contract.DoctorCorporationContractLinkId,
                            "Insert"
                            ),
                        AuditLog.AddLog(
                            "DoctorCorporationContractLinks",
                            "EffectiveDate",
                            null,
                            contract.EffectiveDate.ToString(),
                            contract.DoctorCorporationContractLinkId,
                            "Insert"
                            )
                    });
                }
            }
            return auditLogs;
        }

        public IEnumerable<AuditLog> SaveLinkedContracts(IEnumerable<DoctorCorporationContractLink> contracts)
        {
            return SaveItems(contracts,
                (collection, item) => collection.Any(c =>
                c.DoctorCorporationContractLinkId == item.DoctorCorporationContractLinkId));
        }

        public IEnumerable<AuditLog> RemoveLinkedContracts(List<DoctorCorporationContractLink> doctorCorporationContractLinks)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var linkedContract in doctorCorporationContractLinks)
            {
                auditLogs.AddRange(new List<AuditLog>
                {
                    AuditLog.AddLog(
                        "DoctorCorporationContractLinks",
                        "ContractLineofBusinessId",
                        linkedContract.ContractLineofBusinessId.ToString(),
                        null,
                        linkedContract.DoctorCorporationContractLinkId,
                        "Delete"),

                    AuditLog.AddLog(
                        "DoctorCorporationContractLinks",
                        "DoctorId",
                        linkedContract.DoctorId.ToString(),
                        null,
                        linkedContract.DoctorCorporationContractLinkId,
                        "Delete"),

                    AuditLog.AddLog(
                        "DoctorCorporationContractLinks",
                        "Note",
                        linkedContract.Note,
                        null,
                        linkedContract.DoctorCorporationContractLinkId,
                        "Delete"),

                    AuditLog.AddLog(
                        "DoctorCorporationContractLinks",
                        "EffectiveDate",
                        linkedContract.EffectiveDate.ToString(),
                        null,
                        linkedContract.DoctorCorporationContractLinkId,
                        "Delete")
                });

                Remove(linkedContract);
            }
            return auditLogs;
        }
    }
}