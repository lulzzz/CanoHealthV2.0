using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize]
    public class SchedulersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private ISchedulerEventService

        public SchedulersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Schedulers
        public ActionResult Index()
        {
            return View("Scheduler");
        }

        public ActionResult ReadSchedules([DataSourceRequest] DataSourceRequest request)
        {
            var schedules = _unitOfWork.ScheduleRepository
                .GetScheduleDetails()
                .Select(ScheduleViewModel.Wrap)
                .ToList();

            return Json(schedules.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}