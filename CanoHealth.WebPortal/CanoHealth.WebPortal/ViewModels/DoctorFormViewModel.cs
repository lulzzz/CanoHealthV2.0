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
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(255)]
        public string Degree { get; set; }

        [Required]
        [StringLength(11)]
        [Display(Name = "Social Security Number")]
        public string SocialSecurityNumber { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "NPI")]
        public string NpiNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "CAQH")]
        public string CaqhNumber { get; set; }

        public bool Active { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}