using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class ContractLineofBusiness
    {
        public Guid ContractLineofBusinessId { get; set; }

        public Guid ContractId { get; set; }

        public Guid PlanTypeId { get; set; }

        //Navegation Properties
        public Contract Contract { get; set; }

        public PlanType LineOfBusiness { get; set; }

        public ICollection<ClinicLineofBusinessContract> ClinicLineofBusiness { get; set; }

        public ContractLineofBusiness()
        {
            ClinicLineofBusiness = new Collection<ClinicLineofBusinessContract>();
        }

        public ContractBusinessLineModifyViewModel Modify(ContractLineofBusiness contractBusinessLine)
        {
            var auditLogs = new List<AuditLog>();
            var clinicContractLineofBusinessToDelete = new List<ClinicLineofBusinessContract>();
            if (ContractId != contractBusinessLine.ContractId)
            {
                auditLogs.Add(AuditLog.AddLog("ContractLineofBusiness", "ContractId", ContractId.ToString(), contractBusinessLine.ContractId.ToString(), ContractLineofBusinessId, "Update"));
                ContractId = contractBusinessLine.ContractId;
            }

            if (PlanTypeId != contractBusinessLine.PlanTypeId)
            {
                auditLogs.Add(AuditLog.AddLog("ContractLineofBusiness", "PlanTypeId", PlanTypeId.ToString(), contractBusinessLine.PlanTypeId.ToString(), ContractLineofBusinessId, "Update"));
                PlanTypeId = contractBusinessLine.PlanTypeId;
            }

            var clinicLineofBusinessByParam = contractBusinessLine.ClinicLineofBusiness
                .Select(x => new
                {
                    x.ContractLineofBusinessId,
                    x.PlaceOfServiceId,
                    x.Id
                }).ToList();

            var clinicLineofBusinessCurrent = ClinicLineofBusiness
                .Select(x => new
                {
                    x.ContractLineofBusinessId,
                    x.PlaceOfServiceId,
                    x.Id
                }).ToList();

            var toAdd = clinicLineofBusinessByParam
                .Except(clinicLineofBusinessCurrent)
                .Select(x => new ClinicLineofBusinessContract
                {
                    ContractLineofBusinessId = x.ContractLineofBusinessId,
                    PlaceOfServiceId = x.PlaceOfServiceId,
                    Id = x.Id,
                    ContractLineofBusiness = this
                })
                .ToList();

            var toDelete = clinicLineofBusinessCurrent
                .Except(clinicLineofBusinessByParam)
                .ToList();

            ClinicLineofBusiness.AddRange(toAdd);

            foreach (var item in toDelete)
            {
                var foundToDelete = ClinicLineofBusiness.SingleOrDefault(x => x.Id == item.Id
                                    && x.ContractLineofBusinessId == item.ContractLineofBusinessId
                                    && x.PlaceOfServiceId == item.PlaceOfServiceId);
                if (foundToDelete != null)
                {
                    clinicContractLineofBusinessToDelete.Add(foundToDelete);
                    auditLogs.Add(AuditLog.AddLog("ClinicLineofBusinessContract", "ContractLineofBusinessId",
                        foundToDelete.ContractLineofBusinessId.ToString(), null, foundToDelete.Id, "Delete"));
                    auditLogs.Add(AuditLog.AddLog("ClinicLineofBusinessContract", "PlaceOfServiceId",
                        foundToDelete.PlaceOfServiceId.ToString(), null,
                        foundToDelete.Id, "Delete"));
                }
            }

            var newClinicLineofBusinessContractLogs = toAdd.Select(x => new List<AuditLog>
            {
               AuditLog.AddLog("ClinicLineofBusinessContract", "ContractLineofBusinessId", null,
                    x.ContractLineofBusinessId.ToString(), x.Id, "Insert"),
                AuditLog.AddLog("ClinicLineofBusinessContract", "PlaceOfServiceId", null, x.PlaceOfServiceId.ToString(),
                    x.Id, "Insert")
            });

            foreach (var item in newClinicLineofBusinessContractLogs)
            {
                auditLogs.AddRange(item);
            }

            return new ContractBusinessLineModifyViewModel
            {
                Logs = auditLogs,
                Cliniccontractlineofbusiness = clinicContractLineofBusinessToDelete
            };
        }
    }
}