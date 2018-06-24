using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Core.Repositories;
using CanoHealth.WebPortal.ViewModels;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class SearchCredentialsRepository : Repository<SearchCredentialsResultViewModel>, ISearchCredentialsRepository
    {
        public SearchCredentialsRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<SearchResultDoctorInfoDto> GetSearchResultsByInsuranceAndLocation(Guid corporationId, Guid insuranceId, Guid locationId)
        {
            var query = "EXEC [dbo].[SearchByCorporationInsuranceLocationSql] @corporationId, @insuranceId, @placeOfServiceId";
            var result = GetWithRawSqlForTypesAreNotEntities(query,
                    new SqlParameter("@corporationId", SqlDbType.UniqueIdentifier) { Value = corporationId },
                    new SqlParameter("@insuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId },
                    new SqlParameter("@placeOfServiceId", SqlDbType.UniqueIdentifier) { Value = locationId }
                ).ToList();

            var doctors = ConvertToDoctorInfo(result).ToList();

            //return result;
            return doctors;
        }

        public IEnumerable<SearchResultLocationInfoDto> GetSearchResultsByInsuranceAndDoctor(Guid corporationId, Guid insuranceId, Guid doctorId)
        {
            var query = "EXEC [dbo].[SearchBycorporationInsuranceDoctorSql] @corporationId, @insuranceId, @doctorId";
            var result = GetWithRawSqlForTypesAreNotEntities(query,
                    new SqlParameter("@corporationId", SqlDbType.UniqueIdentifier) { Value = corporationId },
                    new SqlParameter("@insuranceId", SqlDbType.UniqueIdentifier) { Value = insuranceId },
                    new SqlParameter("@doctorId", SqlDbType.UniqueIdentifier) { Value = doctorId }
                ).ToList();

            //return result;
            var locations = ConvertToLocationInfo(result);
            return locations;
        }

        private List<SearchResultDoctorInfoDto> ConvertToDoctorInfo(IEnumerable<SearchCredentialsResultViewModel> searchResult)
        {
            var doctorNpis = searchResult.Select(npi => npi.NpiNumber).Distinct().ToList();
            var doctors = new List<SearchResultDoctorInfoDto>();

            foreach (var npi in doctorNpis)
            {
                var doctor = searchResult.Select(d => new SearchResultDoctorInfoDto
                {
                    Degree = d.Degree,
                    FullName = d.FullName,
                    DateOfBirth = d.DateOfBirth,
                    NpiNumber = d.NpiNumber,
                    CaqhNumber = d.CaqhNumber
                }).FirstOrDefault(x => x.NpiNumber == npi);
                if (doctor != null)
                    doctors.Add(doctor);
            }

            foreach (var doctor in doctors)
            {
                var lineOfBusiness = searchResult.Where(sr => sr.NpiNumber == doctor.NpiNumber)
                    .Select(lb =>
                        new DoctorLinkedToLineOfBusinessDto
                        {
                            LineOfBusinessName = lb.LineOfBusiness,
                            EffectiveDate = lb.EffectiveDate,
                            Note = lb.Note
                        })
                    .ToList();
                doctor.LineOfBusiness = lineOfBusiness;
            }
            return doctors;
        }

        private List<SearchResultLocationInfoDto> ConvertToLocationInfo(IEnumerable<SearchCredentialsResultViewModel> searchResult)
        {
            var locationIds = searchResult.Select(sr => sr.PlaceOfServiceId).Distinct().ToList();
            var locations = new List<SearchResultLocationInfoDto>();

            foreach (var loc in locationIds)
            {
                var location = searchResult.Select(l => new SearchResultLocationInfoDto
                {
                    PlaceOfServiceId = l.PlaceOfServiceId,
                    Location = l.Location,
                    PhoneNumber = l.PhoneNumber,
                    FaxNumber = l.FaxNumber,
                    Address = l.Address
                }).FirstOrDefault(x => x.PlaceOfServiceId == loc);
                if (location != null)
                    locations.Add(location);
            }

            foreach (var location in locations)
            {
                var lineOfBusiness = searchResult.Where(sr => sr.PlaceOfServiceId == location.PlaceOfServiceId).Select(lb =>
                        new DoctorLinkedToLineOfBusinessDto
                        {
                            LineOfBusinessName = lb.LineOfBusiness,
                            EffectiveDate = lb.EffectiveDate,
                            Note = lb.Note
                        })
                    .ToList();
                location.LineOfBusiness = lineOfBusiness;
            }
            return locations;
        }
    }
}