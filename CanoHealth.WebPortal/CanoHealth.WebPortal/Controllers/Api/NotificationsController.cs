using CanoHealth.WebPortal.Core;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class NotificationsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetNotifications()
        {
            var result = _unitOfWork.ExpireDateNotificationRepository
                .GetNotifications()
                .ToList();
            return Ok(result);
        }
    }
}
