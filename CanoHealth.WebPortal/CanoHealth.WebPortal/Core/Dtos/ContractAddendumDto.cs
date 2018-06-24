using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ContractAddendumDto
    {
        public Guid ContractAddendumId { get; set; }

        public Guid ContractId { get; set; }

        public string UniqueFileName { get; set; }

        public string OriginalFileName { get; set; }

        public string FileExtension { get; set; }

        public string FileSize { get; set; }

        public string ContentType { get; set; }

        public string ServerLocation { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string UploadBy { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}