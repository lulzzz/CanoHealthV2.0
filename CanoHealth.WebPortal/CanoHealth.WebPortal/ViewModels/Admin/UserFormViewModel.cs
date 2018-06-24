using IdentitySample.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels.Admin
{
    public class UserFormViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(125)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(125)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool Active { get; set; }

        [Required]
        public IEnumerable<RoleViewModel> Roles { get; set; }

        [Required]
        public IEnumerable<CorporationViewModel> Corporations { get; set; }

        public UserFormViewModel()
        {
            Roles = new List<RoleViewModel>();
            Corporations = new List<CorporationViewModel>();
        }

        public ApplicationUser Convert()
        {
            return new ApplicationUser
            {
                FirstName = FirstName,
                LastName = LastName,
                UserName = Email,
                Email = Email,
                Active = Active
            };
        }
    }
}