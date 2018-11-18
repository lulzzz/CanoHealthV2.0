using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class PersonalFilesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonalFilesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetActivePersonalFiles(Guid? doctorId = null)
        {
            var activePersonalFiles = _unitOfWork.PersonalFileRepository
                .GetActivePersonalFiles(doctorId)
                .Select(Mapper.Map<DoctorFile, DoctorPersonalFileDto>)
                .ToList();
            return Ok(activePersonalFiles);
        }

        [HttpDelete]
        public IHttpActionResult InactivatePersonalFile(DoctorPersonalFileDto personalFileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var personalFileStoreInDb = _unitOfWork.PersonalFileRepository
                    .Get(personalFileDto.DoctorFileId);

                if (personalFileStoreInDb == null)
                {
                    return NotFound();
                }

                var inactivateLog = personalFileStoreInDb.Inactivate();

                _unitOfWork.AuditLogs.Add(inactivateLog);

                _unitOfWork.Complete();

                personalFileDto.Active = personalFileStoreInDb.Active;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(personalFileDto);
        }
    }
}
