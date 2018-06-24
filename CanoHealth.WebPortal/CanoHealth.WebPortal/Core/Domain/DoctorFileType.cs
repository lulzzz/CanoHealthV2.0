using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorFileType
    {
        public Guid DoctorFileTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string DoctorFileTypeName { get; set; }

        //Navegation Properties
        public ICollection<DoctorFile> DoctorFiles { get; set; }

        public DoctorFileType()
        {
            DoctorFiles = new Collection<DoctorFile>();
        }
    }
}