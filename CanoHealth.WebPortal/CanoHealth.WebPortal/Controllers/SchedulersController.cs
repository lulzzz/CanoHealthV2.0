using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
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

        public ActionResult CreateSchedule([DataSourceRequest] DataSourceRequest request,
            ScheduleViewModel schedule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var scheduleToStore = schedule.ConvertToSchedule();
                    _unitOfWork.ScheduleRepository.Add(scheduleToStore);

                    var doctorSchedule = schedule.Doctors
                        .Select(ds => new DoctorSchedule
                        {
                            ScheduleId = scheduleToStore.ScheduleId,
                            DoctorId = ds,
                            DoctorScheduleId = Guid.NewGuid()
                        }).ToList();
                    _unitOfWork.DoctorScheduleRepository.AddRange(doctorSchedule);
                    _unitOfWork.Complete();
                    schedule.ScheduleId = scheduleToStore.ScheduleId;
                }

                return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }
        }
    }
}