using System;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace CanoHealth.WebPortal.ViewModels
{
    public class DoctorFormViewModel
    {
        [BindNever]
        public Guid? DoctorId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(255)]
        public string Degree { get; set; }

        [Required]
        [StringLength(11)]
        public string SocialSecurityNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string NpiNumber { get; set; }

        [StringLength(20)]
        public string CaqhNumber { get; set; }

        public bool Active { get; set; }
    }
}