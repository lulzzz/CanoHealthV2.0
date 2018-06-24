using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels.Admin
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

        public bool Active { get; set; }
    }
}