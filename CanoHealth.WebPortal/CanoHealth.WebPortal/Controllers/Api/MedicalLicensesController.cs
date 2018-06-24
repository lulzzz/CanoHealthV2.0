using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class MedicalLicensesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalLicensesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetMedicalLicenses(Guid? doctorId = null)
        {
            var result = _unitOfWork.MedicalLicenses.GetActiveMedicalLicenses(doctorId)
                         .Select(MedicalLicenseDto.Wrap);

            return Ok(result);
        }

        [HttpDelete]
        public IHttpActionResult InactivateMedicalLicense(MedicalLicenseDto medicalLicense)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var medicalLicenseStoredInDb = _unitOfWork.MedicalLicenses.Get(medicalLicense.MedicalLicenseId);

                if (medicalLicenseStoredInDb == null)
                    return NotFound();

                var log = medicalLicenseStoredInDb.Inactivate();

                _unitOfWork.AuditLogs.Add(log);

                _unitOfWork.Complete();

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(medicalLicense);
        }
    }
}
