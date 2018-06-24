using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class DoctorFileFormViewModel
    {
        public Guid? DoctorFileId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid? DoctorFileTypeId { get; set; }

        [Required]
        public string DoctorFileTypeName { get; set; }

        [StringLength(100)]
        public string OriginalFileName { get; set; }

        [StringLength(100)]
        public string UniqueFileName { get; set; }

        [StringLength(10)]
        public string FileExtension { get; set; }

        [StringLength(10)]
        public string FileSize { get; set; }

        [StringLength(255)]
        public string ContentType { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string UploadBy { get; set; }

        [StringLength(100)]
        public string ServerLocation { get; set; }

        public bool Active { get; set; }

        public DoctorFile Convert()
        {
            var doctorFile = new DoctorFile
            {
                DoctorFileId = DoctorFileId ?? Guid.NewGuid(),
                DoctorId = DoctorId,
                DoctorFileTypeId = DoctorFileTypeId.Value,
                OriginalFileName = OriginalFileName,
                UniqueFileName = UniqueFileName ?? Guid.NewGuid() + FileExtension,
                FileExtension = FileExtension,
                FileSize = FileSize,
                ContentType = ContentType,
                UploadDateTime = UploadDateTime,
                UploadBy = UploadBy,
                ServerLocation = ConfigureSettings.GetPersonalFilesDirectory,
                Active = Active
            };
            return doctorFile;
        }
    }
}