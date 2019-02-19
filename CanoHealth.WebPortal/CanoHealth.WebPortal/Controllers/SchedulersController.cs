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
    [Authorize(Roles = "ADMIN,SCHEDULER")]
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

        [AllowAnonymous]
        public ActionResult ReadSchedules([DataSourceRequest] DataSourceRequest request, Guid? doctorId = null)
        {
            var schedules = new List<ScheduleViewModel>();

            if (doctorId == null || doctorId == Guid.Empty)
            {
                schedules = _unitOfWork.ScheduleRepository
                   .GetSchedules()
                   .Select(ScheduleViewModel.Wrap)
                   .ToList();
            }
            else
            {
                schedules = _unitOfWork.DoctorScheduleRepository
                    .GetSchedulesByDoctorId(doctorId.Value)
                    .Select(s => s.Schedule)
                    .Select(ScheduleViewModel.Wrap)
                    .ToList();
            }

            return Json(schedules.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ReadScheduleJson(Guid? doctorId = null)
        {
            var schedules = new List<ScheduleViewModel>();

            if (doctorId == null || doctorId == Guid.Empty)
            {
                schedules = _unitOfWork.ScheduleRepository
                   .GetSchedules()
                   .Select(ScheduleViewModel.Wrap)
                   .ToList();
            }
            else
            {
                schedules = _unitOfWork.DoctorScheduleRepository
                    .GetSchedulesByDoctorId(doctorId.Value)
                    .Select(s => s.Schedule)
                    .Select(ScheduleViewModel.Wrap)
                    .ToList();
            }
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSchedule([DataSourceRequest] DataSourceRequest request,
            ScheduleViewModel schedule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //get the list of doctors already scheduled for the date passed as parameter
                    var doctorschedulesfound = _unitOfWork.DoctorScheduleRepository.GetDoctorSchedules(schedule);

                    if (doctorschedulesfound.Any())
                    {
                        var doctors = doctorschedulesfound.Select(d => d.Doctor.GetFullName()).ToList();

                        var message = $"Doctors already scheduled on {schedule.Start.ToShortDateString()}: {String.Join(",", doctors)}.";

                        //get the list of doctors that are not scheduled yet for that date
                        schedule.Doctors = schedule.Doctors.Except(doctorschedulesfound.Select(d => d.DoctorId)).ToList();

                        if (!schedule.Doctors.Any())
                        {
                            ModelState.AddModelError("Doctors", $"{message} The Doctors field is required.");
                            return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
                        }
                        else
                            ModelState.AddModelError("", message);
                    }

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
                ModelState.AddModelError("CancelChanges", "We are sorry, but something went wrong. Please try again.");
            }
            return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSchedule([DataSourceRequest] DataSourceRequest request,
            ScheduleViewModel schedule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Get the detailed schedule with the collection of DoctorSchedule
                    var scheduleStoredInDb = _unitOfWork.ScheduleRepository.GetDetailedSchedule(schedule.ScheduleId.Value);
                    if (scheduleStoredInDb == null)
                    {
                        ModelState.AddModelError("CancelChanges", "Schedule not found. Please try again.");
                        return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
                    }

                    //get the list of doctors already scheduled for the date passed as parameter
                    var doctorschedulesfound = _unitOfWork.DoctorScheduleRepository.GetDoctorSchedules(schedule);

                    if (doctorschedulesfound.Any())
                    {
                        var doctors = doctorschedulesfound.Select(d => d.Doctor.GetFullName()).ToList();

                        var message = $"Doctors already scheduled on {schedule.Start.ToShortDateString()}: {String.Join(",", doctors)}.";

                        //get the list of doctors that are not scheduled yet for that date
                        schedule.Doctors = schedule.Doctors.Except(doctorschedulesfound.Select(d => d.DoctorId)).ToList();

                        if (!schedule.Doctors.Any())
                        {
                            ModelState.AddModelError("Doctors", $"{message} The Doctors field is required.");
                            return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
                        }
                        else
                            ModelState.AddModelError("", message);
                    }

                    var scheduleByParam = schedule.ConvertToSchedule();

                    //update the fields of the detailed schedule with
                    scheduleStoredInDb.Modify(scheduleByParam);

                    //Clean the current DoctorSchedules from the Schedule found
                    _unitOfWork.DoctorScheduleRepository.RemoveRange(scheduleStoredInDb.DoctorSchedules);

                    //Create new instances of DoctorSchedule object
                    var doctorSchedules = schedule.Doctors
                                        .Select(ds => new DoctorSchedule
                                        {
                                            ScheduleId = scheduleStoredInDb.ScheduleId,
                                            DoctorId = ds,
                                            DoctorScheduleId = Guid.NewGuid()
                                        }).ToList();
                    _unitOfWork.DoctorScheduleRepository.AddRange(doctorSchedules);

                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("CancelChanges", "We are sorry, but something went wrong. Please try again.");
            }
            return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSchedule([DataSourceRequest] DataSourceRequest request,
            ScheduleViewModel schedule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Check if the schedule item exist in our DB
                    var scheduleStoredInDb = _unitOfWork.ScheduleRepository.Get(schedule.ScheduleId.Value);
                    if (scheduleStoredInDb == null)
                    {
                        ModelState.AddModelError("CancelChanges", "Schedule not found. Please try again.");
                        return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
                    }

                    var doctorschedulefound = _unitOfWork.DoctorScheduleRepository
                        .EnumarableGetAll(ds => ds.ScheduleId == schedule.ScheduleId && schedule.Doctors.Contains(ds.DoctorId)
                    ).ToList();

                    var recurrenceExceptions = _unitOfWork.ScheduleRepository.EnumarableGetAll(m => m.RecurrenceID == schedule.ScheduleId).ToList();

                    //audit logs
                    var auditLogs = _doctorScheduleLog.GenerateLogsWhenDelete(doctorschedulefound).ToList();
                    auditLogs.AddRange(_scheduleLog.GenerateLogsWhenDelete(recurrenceExceptions));
                    auditLogs.AddRange(_scheduleLog.GenerateLogsWhenDelete(new List<Schedule> { scheduleStoredInDb }));

                    _unitOfWork.ScheduleRepository.RemoveRange(recurrenceExceptions);

                    _unitOfWork.DoctorScheduleRepository.RemoveRange(doctorschedulefound);

                    _unitOfWork.ScheduleRepository.Remove(scheduleStoredInDb);

                    _unitOfWork.AuditLogs.AddRange(auditLogs);

                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("CancelChanges", "We are sorry, but something went wrong. Please try again.");
            }

            return Json(new[] { schedule }.ToDataSourceResult(request, ModelState));
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