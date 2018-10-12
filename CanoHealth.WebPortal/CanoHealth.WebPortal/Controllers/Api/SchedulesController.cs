using CanoHealth.WebPortal.Persistance;
using Elmah;
using System;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class SchedulesController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public SchedulesController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetSchedule(Guid scheduleId)
        {
            try
            {
                var schedule = _unitOfWork.ScheduleRepository.GetDetailedSchedule(scheduleId);
                if (schedule == null)
                    return NotFound();

                return Ok(schedule.ConvertToDto());
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }
    }
}
