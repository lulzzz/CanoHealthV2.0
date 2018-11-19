using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IClinicDoctorRepository : IRepository<DoctorClinic>
    {
        IEnumerable<Doctor> GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(Guid placeOfServiceId);
        DoctorClinic FindDoctorClinicRelationship(Guid doctorId, Guid placeOfServiceId);
        IEnumerable<AuditLog> AssignDoctorsToClinic(List<DoctorClinic> doctorClinic);
        IEnumerable<PlaceOfService> GetLocationsWhereDoctorWorks(Guid doctorId);

        Task<IEnumerable<DoctorClinic>> GetDoctorLocationsAsync(Guid doctorId);
    }
}