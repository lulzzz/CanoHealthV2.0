using CanoHealth.WebPortal.CommonTools.ExpireLicense;
using CanoHealth.WebPortal.CommonTools.Roles;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [OutputCache(Duration = 3600)]
        public ActionResult GetNotifications()
        {
            var result = new List<NotificationDto>();
            if (User.IsInRole(RoleName.Admin))
            {
                result = _unitOfWork.ExpireDateNotificationRepository
                    .GetNotifications()
                    .Where(x => !x.Source.Equals(ExpireLicenseSource.OutOfNetworkContracts, System.StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
