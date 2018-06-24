using Microsoft.AspNet.Identity.EntityFramework;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class ApplicationRole : IdentityRole
    {
        public bool Active { get; set; }

        public ApplicationRole() : base() { }

        public ApplicationRole(string name, bool active) : base(name)
        {
            this.Active = active;
        }
    }
}