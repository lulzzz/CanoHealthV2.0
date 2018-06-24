using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class ContractAddendumFormViewModel
    {
        public Guid? ContractAddendumId { get; set; }

        public Guid ContractId { get; set; }

        [StringLength(100)]
        public string UniqueFileName { get; set; }

        [Required]
        [StringLength(100)]
        public string OriginalFileName { get; set; }

        [Required]
        [StringLength(10)]
        public string FileExtension { get; set; }

        [Required]
        [StringLength(10)]
        public string FileSize { get; set; }

        [Required]
        [StringLength(255)]
        public string ContentType { get; set; }

        [StringLength(100)]
        public string ServerLocation { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string UploadBy { get; set; }

        [StringLength(3000)]
        public string Description { get; set; }

        public bool Active { get; set; }

        public ContractAddendum Convert()
        {
            return new ContractAddendum
            {
                ContractAddendumId = ContractAddendumId ?? Guid.NewGuid(),
                ContractId = ContractId,
                UniqueFileName = UniqueFileName ?? Guid.NewGuid() + FileExtension,
                OriginalFileName = OriginalFileName,
                FileSize = FileSize,
                FileExtension = FileExtension,
                ContentType = ContentType,
                ServerLocation = ConfigureSettings.GetContractAddendumsDirectory,
                Description = Description,
                Active = Active
            };
        }
    }
}