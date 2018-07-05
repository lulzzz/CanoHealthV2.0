using CanoHealth.WebPortal.CommonTools.ExpireLicense;
using CanoHealth.WebPortal.CommonTools.Roles;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    [Authorize]
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
            var result = new List<NotificationDto>();
            if (User.IsInRole(RoleName.Admin))
            {
                result = _unitOfWork.ExpireDateNotificationRepository
                    .GetNotifications()
                    .Where(x => !x.Source.Equals(ExpireLicenseSource.OutOfNetworkContracts, System.StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            return Ok(result);
        }
    }
}
