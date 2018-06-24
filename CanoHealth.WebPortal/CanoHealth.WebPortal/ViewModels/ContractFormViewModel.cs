using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class ContractFormViewModel
    {
        public Guid? ContractId { get; set; }

        [Display(Name = "Corporation")]
        public Guid CorporationId { get; set; }

        public Guid? InsuranceId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Insurance Name")]
        public string InsuranceName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Group Number")]
        public string GroupNumber { get; set; }

        public bool Active { get; set; }

        public string OriginalFileName { get; set; }

        public string FileExtension { get; set; }

        public string FileSize { get; set; }

        public string ContentType { get; set; }

        public bool? FileActive { get; set; }

        public static ContractFormViewModel Wrap(Contract contract)
        {
            return new ContractFormViewModel
            {
                ContractId = contract.ContractId,
                CorporationId = contract.CorporationId,
                InsuranceId = contract.InsuranceId,
                InsuranceName = contract.Insurance.Name,
                GroupNumber = contract.GroupNumber,
                Active = contract.Active
            };
        }

        public Contract ConvertToContract()
        {
            return new Contract
            {
                ContractId = ContractId ?? Guid.NewGuid(),
                CorporationId = CorporationId,
                InsuranceId = InsuranceId.Value,
                GroupNumber = GroupNumber,
                Active = Active
            };
        }

        public ContractAddendum ContractAddendum(string user, DateTime dateTime)
        {
            return new ContractAddendum
            {
                ContractAddendumId = Guid.NewGuid(),
                ContractId = ContractId.Value,
                Active = true,
                OriginalFileName = OriginalFileName,
                UniqueFileName = Guid.NewGuid() + FileExtension,
                FileExtension = FileExtension,
                FileSize = FileSize,
                ContentType = ContentType,
                ServerLocation = ConfigureSettings.GetContractAddendumsDirectory,
                UploadBy = user,
                UploadDateTime = dateTime
            };
        }

        public Insurance ConvertToInsurance()
        {
            return new Insurance
            {
                InsuranceId = InsuranceId.Value,
                Name = InsuranceName,
                Active = true
            };
        }
    }
}