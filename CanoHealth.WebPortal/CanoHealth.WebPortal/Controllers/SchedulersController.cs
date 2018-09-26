using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Services.AuditLogs;
using CanoHealth.WebPortal.Services.AuditLogs.DoctorSchedules;
using CanoHealth.WebPortal.Services.AuditLogs.Schedules;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize]
    public class SchedulersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDoctorScheduleLog _doctorScheduleLog;
        private readonly IScheduleLog _scheduleLog;
        private readonly ILogs<DoctorSchedule> _logs;

        //private ISchedulerEventService

        public SchedulersController(IUnitOfWork unitOfWork, IDoctorScheduleLog doctorScheduleLog, IScheduleLog scheduleLog, ILogs<DoctorSchedule> logs)
        {
            _unitOfWork = unitOfWork;
            _doctorScheduleLog = doctorScheduleLog;
            _scheduleLog = scheduleLog;
            _logs = logs;
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
                    //Create an instance of Schedule object
                    var scheduleToStore = schedule.ConvertToSchedule();
                    _unitOfWork.ScheduleRepository.Add(scheduleToStore);

                    //Create instances of DoctorSchedule object
                    var doctorSchedules = schedule.Doctors
                                        .Select(ds => new DoctorSchedule
                                        {
                                            ScheduleId = scheduleToStore.ScheduleId,
                                            DoctorId = ds,
                                            DoctorScheduleId = Guid.NewGuid()
                                        }).ToList();
                    _unitOfWork.DoctorScheduleRepository.AddRange(doctorSchedules);

                    //Create logs                    
                    var auditLogs = _doctorScheduleLog.GenerateLogs(doctorSchedules).ToList();
                    //var auditLogs = _logs.GenerateLogs(doctorSchedules).ToList();

                    auditLogs.AddRange(_scheduleLog.GenerateLogs(new List<Schedule> { scheduleToStore }));

                    _unitOfWork.AuditLogs.AddRange(auditLogs);

                    _unitOfWork.Complete();
                    schedule.ScheduleId = scheduleToStore.ScheduleId;
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, String.Format("An exception has occurred: {0}", ex));
                ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again.");
            }
            return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult UpdateSchedule([DataSourceRequest] DataSourceRequest request,
            ScheduleViewModel schedule)
        {
            return Content("");
        }

        public ActionResult DeleteSchedule()
        {
            return Content("");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExportSchedule(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    }
}