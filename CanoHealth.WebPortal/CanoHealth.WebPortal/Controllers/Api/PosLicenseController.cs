using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class PosLicenseController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PosLicenseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetLicenses(string placeOfServiceId = null)
        {
            try
            {
                var licenses = _unitOfWork.Licenses.GetActiveLicenses(placeOfServiceId)
                                          .Select(LicenseDto.Wrap).ToList();
                return Ok(licenses);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        public IHttpActionResult InactiveLicense(LicenseDto licenseDto)
        {
            try
            {
                var license = _unitOfWork.Licenses.Get(licenseDto.PosLicenseId);

                if (license == null)
                    return NotFound();
                var auditLog = license.InactiveLicense();

                _unitOfWork.AuditLogs.Add(auditLog);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok();
        }
    }
}
