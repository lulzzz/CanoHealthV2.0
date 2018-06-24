using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ClinicDoctorRepository : Repository<DoctorClinic>, IClinicDoctorRepository
    {
        public ClinicDoctorRepository(ApplicationDbContext context) : base(context) { }

        /*Todos los doctores que trabajan en esta clinica, estan activos*/
        public IEnumerable<Doctor> GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(Guid placeOfServiceId)
        {
            var doctors = EnumarableGetAll(dc => dc.PlaceOfServiceId == placeOfServiceId && dc.Active,
                            includeProperties: new Expression<Func<DoctorClinic, object>>[] { d => d.Doctor })
                            .Select(d => d.Doctor)
                            //.Where(d => d.Active)
                            .ToList();
            return doctors;
        }

        public IEnumerable<PlaceOfService> GetLocationsWhereDoctorWorks(Guid doctorId)
        {
            var locations = EnumarableGetAll(x => x.DoctorId == doctorId && x.Active,
                includeProperties: new Expression<Func<DoctorClinic, object>>[]
                {
                    pos => pos.Clinic
                }).Select(x => x.Clinic).ToList();
            return locations;
        }

        public DoctorClinic FindDoctorClinicRelationship(Guid doctorId, Guid placeOfServiceId)
        {
            return SingleOrDefault(dc => dc.DoctorId == doctorId && dc.PlaceOfServiceId == placeOfServiceId);
        }

        public IEnumerable<AuditLog> AssignDoctorsToClinic(List<DoctorClinic> doctorClinic)
        {
            return SaveItems(doctorClinic, (collection, clinic) => collection.Any(dc => dc.DoctorClinicId == clinic.DoctorClinicId));
        }

        private IEnumerable<AuditLog> SaveItems(List<DoctorClinic> doctorClinic, Func<DbSet<DoctorClinic>, DoctorClinic, bool> existItem)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var item in doctorClinic)
            {
                if (existItem(Entities, item))
                {
                    var storedInDb = Get(item.DoctorClinicId);
                    if (storedInDb.PlaceOfServiceId != item.PlaceOfServiceId)
                    {
                        auditLogs.Add(AuditLog.AddLog("DoctorClinics", "PlaceOfServiceId", storedInDb.PlaceOfServiceId.ToString(), item.PlaceOfServiceId.ToString(), item.DoctorClinicId, "Update"));
                        storedInDb.PlaceOfServiceId = item.PlaceOfServiceId;
                    }
                    if (storedInDb.DoctorId != item.DoctorId)
                    {
                        auditLogs.Add(AuditLog.AddLog("DoctorClinics", "DoctorId", storedInDb.DoctorId.ToString(), item.DoctorId.ToString(), item.DoctorClinicId, "Update"));
                        storedInDb.DoctorId = item.DoctorId;
                    }
                    if (storedInDb.Active != item.Active)
                    {
                        auditLogs.Add(AuditLog.AddLog("DoctorClinics", "Active", storedInDb.Active.ToString(), item.Active.ToString(), item.DoctorClinicId, "Update"));
                        storedInDb.Active = item.Active;
                    }
                }
                else
                {
                    Add(item);
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog("DoctorClinics", "PlaceOfServiceId", null, item.PlaceOfServiceId.ToString(), item.DoctorClinicId, "Insert"),
                        AuditLog.AddLog("DoctorClinics", "DoctorId", null, item.DoctorId.ToString(), item.DoctorClinicId, "Update"),
                        AuditLog.AddLog("DoctorClinics", "Active", null, item.Active.ToString(), item.DoctorClinicId, "Update")
                    });
                }
            }
            return auditLogs;
        }
    }
}