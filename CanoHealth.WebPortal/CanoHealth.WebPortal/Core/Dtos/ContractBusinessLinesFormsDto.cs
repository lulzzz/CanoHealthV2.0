using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ContractBusinessLinesFormsDto
    {
        public Guid ContractLineofBusinessId { get; set; }

        //Contrato Info
        public Guid ContractId { get; set; }

        //Business Lines info
        public Guid PlanTypeId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        [Required]
        public IEnumerable<ClinicLineofBusinessContractDto> Clinics { get; set; }

        public ContractBusinessLinesFormsDto()
        {
            Clinics = new List<ClinicLineofBusinessContractDto>();
        }

        public ContractLineofBusiness CreateContractBusinessLineItem()
        {
            return new ContractLineofBusiness
            {
                ContractLineofBusinessId = ContractLineofBusinessId,
                ContractId = ContractId,
                PlanTypeId = PlanTypeId,
                Active = true
            };
        }

        public IEnumerable<ClinicLineofBusinessContract> CreateContractBusinessLinesClinicsItems()
        {
            var result = Clinics.Select(x => new ClinicLineofBusinessContract
            {
                Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                ContractLineofBusinessId = ContractLineofBusinessId,
                PlaceOfServiceId = x.PlaceOfServiceId,
                Active = true
            }).ToList();
            return result;
        }

        public IEnumerable<AuditLog> GetLogsForCreation(IEnumerable<ClinicLineofBusinessContract> clinicLineofBusinessContract)
        {
            var auditLog = new List<AuditLog>();
            auditLog.AddRange(new List<AuditLog>
            {
                AuditLog.AddLog("ContractLineofBusiness", "ContractId", null, ContractId.ToString(), ContractLineofBusinessId, "Insert"),
                AuditLog.AddLog("ContractLineofBusiness", "PlanTypeId", null, PlanTypeId.ToString(), ContractLineofBusinessId, "Insert")
            });
            var clinicLogs = clinicLineofBusinessContract.Select(x => new List<AuditLog>
            {
                AuditLog.AddLog("ClinicLineofBusinessContracts", "ContractLineofBusinessId", null, x.ContractLineofBusinessId.ToString(), x.Id, "Insert"),
                AuditLog.AddLog("ClinicLineofBusinessContracts", "PlaceOfServiceId", null, x.PlaceOfServiceId.ToString(), x.Id, "Insert"),
            });

            foreach (var item in clinicLogs)
            {
                auditLog.AddRange(item);
            }
            return auditLog;
        }

        public static ClinicLineofBusinessContractDto WrapClinic(ClinicLineofBusinessContract clinic)
        {
            return new ClinicLineofBusinessContractDto
            {
                Id = clinic.Id,
                ContractLineofBusinessId = clinic.ContractLineofBusinessId,
                PlaceOfServiceId = clinic.PlaceOfServiceId,
                Name = clinic.Clinic.Name
            };
        }

        public static ContractBusinessLinesFormsDto WrapContractBusinessLines(ContractLineofBusiness contractLineofBusiness)
        {
            return new ContractBusinessLinesFormsDto
            {
                ContractId = contractLineofBusiness.ContractId,
                PlanTypeId = contractLineofBusiness.PlanTypeId,
                Name = contractLineofBusiness.LineOfBusiness.Name,
                ContractLineofBusinessId = contractLineofBusiness.ContractLineofBusinessId,
                Clinics = contractLineofBusiness.ClinicLineofBusiness.Select(c => new ClinicLineofBusinessContractDto
                {
                    Id = c.Id,
                    ContractLineofBusinessId = c.ContractLineofBusinessId,
                    PlaceOfServiceId = c.PlaceOfServiceId,
                    Name = c.Clinic.Name
                })
            };
        }
    }
}