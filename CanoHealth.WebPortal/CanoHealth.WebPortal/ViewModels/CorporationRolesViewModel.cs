using CanoHealth.WebPortal.ViewModels.Admin;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.ViewModels
{
    public class CorporationRolesViewModel
    {
        public List<CorporationViewModel> Corporations { get; set; }

        public List<RoleViewModel> Roles { get; set; }

        public CorporationRolesViewModel()
        {
            Corporations = new List<CorporationViewModel>();
            Roles = new List<RoleViewModel>();
        }
    }
}