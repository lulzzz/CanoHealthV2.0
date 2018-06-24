using CanoHealth.WebPortal.Core.Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace IdentitySample.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Corporation> Corporations { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<PlanType> PlanTypes { get; set; }
        public DbSet<PlaceOfService> PlaceOfServices { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PosLicense> PosLicenses { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractAddendum> Addendums { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<MedicalLicenseType> MedicalLicenseTypes { get; set; }
        public DbSet<MedicalLicense> MedicalLicenses { get; set; }
        public DbSet<DoctorFileType> DoctorFileTypes { get; set; }
        public DbSet<DoctorFile> DoctorFiles { get; set; }
        public DbSet<DoctorClinic> DoctorClinics { get; set; }
        public DbSet<DoctorCorporationContractLink> DoctorCorporationContractLinks { get; set; }
        public DbSet<InsuranceBusinessLine> InsuranceBusinessLines { get; set; }
        public DbSet<DoctorIndividualProvider> IndividualProviders { get; set; }
        public DbSet<OutOfNetworkContract> OutOfNetworkContracts { get; set; }
        public DbSet<ProviderByLocation> ProviderByLocations { get; set; }
        public DbSet<UserCorporationAccess> UserCorporationAccess { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}