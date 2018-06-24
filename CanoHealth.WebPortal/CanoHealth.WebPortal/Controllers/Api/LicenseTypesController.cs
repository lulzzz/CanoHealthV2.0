using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class LicenseTypesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LicenseTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetLicenseTypes()
        {
            var result = _unitOfWork.LicenseTypes.GetAll().Select(Mapper.Map<LicenseType, LicenseTypeDto>);
            return Ok(result);
        }
    }
}
