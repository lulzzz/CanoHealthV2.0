using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor> GetDoctorByIdAsync(Guid doctorId);
        IEnumerable<Doctor> GetAllActiveDoctors();
        IEnumerable<AuditLog> SaveDoctors(IEnumerable<Doctor> doctors);
        Doctor DuplicateSocialSecurityNumber(Guid doctorId, string socialSecurityNumber);
        Doctor DuplicateNationalProviderIdentifier(Guid doctorId, string npiNumber);
        Doctor DuplicateCaqh(Guid doctorId, string caqhNumber);
        Doctor FinDoctor(string fname, string lname, DateTime dob, string ssn, string npi, string caqh);
        IEnumerable<Doctor> GetAllDoctorsInTheSystem();
    }
}