using Microsoft.AspNet.Identity;
using System.Web;

namespace CanoHealth.WebPortal.CommonTools.User
{
    public class UserService : IUserService
    {
        public string GetUserName()
        {
            return HttpContext.Current.User.Identity.GetUserName();
        }
    }
}