using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Doctor> GetDoctorByIdAsync(Guid doctorId)
        {
            return await GetByIdAsync(doctorId);
        }

        public IEnumerable<Doctor> GetAllDoctorsInTheSystem()
        {
            return EnumarableGetAll(orderBy: ioq => ioq.OrderBy(d => d.FirstName)).ToList();
        }

        public IEnumerable<Doctor> GetAllActiveDoctors()
        {
            return EnumarableGetAll(d => d.Active, orderBy: ioq => ioq.OrderBy(d => d.FirstName)).ToList();
        }

        public Doctor DuplicateSocialSecurityNumber(Guid doctorId, string socialSecurityNumber)
        {
            return SingleOrDefault(d => d.SocialSecurityNumber == socialSecurityNumber && d.DoctorId != doctorId);
        }

        public Doctor DuplicateNationalProviderIdentifier(Guid doctorId, string npiNumber)
        {
            return SingleOrDefault(d => d.NpiNumber == npiNumber && d.DoctorId != doctorId);
        }

        public Doctor DuplicateCaqh(Guid doctorId, string caqhNumber)
        {
            return SingleOrDefault(d => d.CaqhNumber == caqhNumber && d.DoctorId != doctorId);
        }

        public Doctor FinDoctor(string fname, string lname, DateTime dob, string ssn, string npi, string caqh)
        {
            var doc = SingleOrDefault(d => d.FirstName.Equals(fname, StringComparison.CurrentCultureIgnoreCase) &&
                       d.LastName.Equals(lname, StringComparison.CurrentCultureIgnoreCase) &&
                       d.DateOfBirth == dob &&
                       d.SocialSecurityNumber == ssn &&
                       d.NpiNumber.Equals(npi, StringComparison.CurrentCultureIgnoreCase) &&
                       d.CaqhNumber.Equals(caqh, StringComparison.CurrentCultureIgnoreCase));
            return doc;
        }

        public IEnumerable<AuditLog> SaveDoctors(IEnumerable<Doctor> doctors)
        {
            return SaveItems(doctors, (doctorcollection, doctor) => doctorcollection.Any(d => d.DoctorId == doctor.DoctorId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<Doctor> doctors, Func<DbSet<Doctor>, Doctor, bool> existDoctor)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var doctor in doctors)
            {
                if (existDoctor(Entities, doctor))
                {
                    var doctorStoredInDb = Get(doctor.DoctorId);
                    if (doctorStoredInDb.FirstName != doctor.FirstName)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "FirstName", doctorStoredInDb.FirstName, doctor.FirstName, doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.FirstName = doctor.FirstName;
                    }
                    if (doctorStoredInDb.LastName != doctor.LastName)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "LastName", doctorStoredInDb.LastName, doctor.LastName, doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.LastName = doctor.LastName;
                    }
                    if (doctorStoredInDb.DateOfBirth != doctor.DateOfBirth)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "DateOfBirth", doctorStoredInDb.DateOfBirth.ToString(), doctor.DateOfBirth.ToString(), doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.DateOfBirth = doctor.DateOfBirth;
                    }
                    if (doctorStoredInDb.SocialSecurityNumber != doctor.SocialSecurityNumber)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "SocialSecurityNumber", doctorStoredInDb.SocialSecurityNumber, doctor.SocialSecurityNumber, doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.SocialSecurityNumber = doctor.SocialSecurityNumber;
                    }
                    if (doctorStoredInDb.NpiNumber != doctor.NpiNumber)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "NpiNumber", doctorStoredInDb.NpiNumber, doctor.NpiNumber, doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.NpiNumber = doctor.NpiNumber;
                    }
                    if (doctorStoredInDb.Degree != doctor.Degree)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "Degree", doctorStoredInDb.Degree, doctor.Degree, doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.Degree = doctor.Degree;
                    }
                    if (doctorStoredInDb.CaqhNumber != doctor.CaqhNumber)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "CaqhNumber", doctorStoredInDb.CaqhNumber, doctor.CaqhNumber, doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.CaqhNumber = doctor.CaqhNumber;
                    }
                    if (doctorStoredInDb.Active != doctor.Active)
                    {
                        auditLogs.Add(AuditLog.AddLog("Doctor", "Active", doctorStoredInDb.Active.ToString(), doctor.Active.ToString(), doctorStoredInDb.DoctorId, "Update"));
                        doctorStoredInDb.Active = doctor.Active;
                    }
                }
                else
                {
                    Add(doctor);
                    auditLogs.Add(AuditLog.AddLog("Doctor", "FirstName", null, doctor.FirstName, doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "LastName", null, doctor.LastName, doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "DateOfBirth", null, doctor.DateOfBirth.ToString(), doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "SocialSecurityNumber", null, doctor.SocialSecurityNumber, doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "NpiNumber", null, doctor.NpiNumber, doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "Degree", null, doctor.Degree, doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "CaqhNumber", null, doctor.CaqhNumber, doctor.DoctorId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("Doctor", "Active", null, doctor.Active.ToString(), doctor.DoctorId, "Insert"));
                }
            }
            return auditLogs;
        }
    }
}