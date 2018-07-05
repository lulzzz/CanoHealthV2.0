using CanoHealth.WebPortal.Core;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    [Authorize(Roles = "ADMIN")]
    public class UsersController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult CheckUserNameAvailability(string username)
        {
            var user = _unitOfWork.UserRepository.GetByUserName(username);
            return Ok(user);
        }
    }
}
