using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class DoctorPersonalFileDto
    {
        public Guid DoctorFileId { get; set; }

        public Guid DoctorId { get; set; }

        //File Classification Ex.: SSN, Driver License, etc.
        public Guid DoctorFileTypeId { get; set; }

        public string DoctorFileTypeName { get; set; }

        public string OriginalFileName { get; set; }

        public string UniqueFileName { get; set; }

        public string FileExtension { get; set; }

        public string FileSize { get; set; }

        public string ContentType { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string UploadBy { get; set; }

        public string ServerLocation { get; set; }

        public bool Active { get; set; }
    }
}