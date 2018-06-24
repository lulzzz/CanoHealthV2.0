using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class PersonalFileTypesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonalFileTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetFileTypes()
        {
            var fileTypes = _unitOfWork.PersonalFileTypeRepository.GetFileTypes()
                                       .Select(Mapper.Map<DoctorFileType, DoctorFileTypeDto>)
                                       .ToList();

            return Ok(fileTypes);
        }
    }
}
