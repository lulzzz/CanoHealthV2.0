using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Repositories;
using CanoHealth.WebPortal.Persistance.Repositories;
using IdentitySample.Models;

namespace CanoHealth.WebPortal.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        //Define an specific Context
        private readonly ApplicationDbContext _context;

        public ICorporationRepository Corporations { get; private set; }

        public IPlaceOfServiceRepository PlaceOfServices { get; private set; }

        public IAuditLogReporitory AuditLogs { get; private set; }

        public ILicenseRepository Licenses { get; private set; }

        public ILicenseTypeRepository LicenseTypes { get; private set; }

        public IInsuranceRepository Insurances { get; private set; }

        public ILineofBusinessRepository LineOfBusinesses { get; private set; }

        public IContractRepository Contracts { get; private set; }

        public IAddendumRepository Addendums { get; private set; }

        public IContractBusinessLineRepository ContracBusinessLineRepository { get; private set; }

        public IContractBusinessLineClinicRepository ContracBusinessLineClinicRepository { get; private set; }

        public IDoctorRepository Doctors { get; private set; }

        public IMedicalLicenseRepository MedicalLicenses { get; private set; }

        public IMedicalLicenseTypeRepository MedicalLicenseTypes { get; private set; }

        public IClinicDoctorRepository ClinicDoctor { get; private set; }

        public IDoctorLinkedContractRepository DoctorLinkedContracts { get; private set; }

        public ILinkedContractRepository LinkedContractStoredProcedures { get; private set; }

        public IInsuranceBusinessLineRepository InsuranceBusinessLineRepository { get; private set; }

        public IIndividualProviderRepository IndividualProviderRepository { get; private set; }

        public IOutofNetworkContractRepository OutofNetworkContractRepository { get; private set; }

        public IProviderByLocationRepository ProviderByLocationRepository { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IRoleRepository RoleRepository { get; private set; }

        public ISearchCredentialsRepository SearchCredentialsRepository { get; private set; }

        public IPersonalFileRepository PersonalFileRepository { get; private set; }

        public IPersonalFileTypeRepository PersonalFileTypeRepository { get; private set; }

        public IUserCorporationAccessRepository UserCorporationAccessRepository { get; private set; }

        public IExpireDateNotificationRepository ExpireDateNotificationRepository { get; private set; }

        public IScheduleRepository ScheduleRepository { get; private set; }

        public IDoctorScheduleRepository DoctorScheduleRepository { get; private set; }

        //Impplement the constructor
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Corporations = new CorporationRepository(_context);
            PlaceOfServices = new PlaceOfServiceRepository(_context);
            AuditLogs = new AuditLogRepository(_context);
            Licenses = new LicenseRepository(_context);
            LicenseTypes = new LicenseTypeRepository(_context);
            Insurances = new InsuranceRepository(_context);
            LineOfBusinesses = new LineofBusinessRepository(_context);
            Contracts = new ContractRepository(_context);
            Addendums = new AddendumRepository(_context);
            ContracBusinessLineRepository = new ContractBusinessLineRepository(_context);
            ContracBusinessLineClinicRepository = new ContractBusinessLineClinicRepository(_context);
            Doctors = new DoctorRepository(_context);
            MedicalLicenses = new MedicalLicenseRepository(_context);
            MedicalLicenseTypes = new MedicalLicenseTypeRepository(_context);
            ClinicDoctor = new ClinicDoctorRepository(_context);
            DoctorLinkedContracts = new DoctorLinkedContractRepository(_context);
            LinkedContractStoredProcedures = new LinkedContractRepository(_context);
            InsuranceBusinessLineRepository = new InsuranceBusinessLineRepository(_context);
            IndividualProviderRepository = new IndividualProviderRepository(_context);
            OutofNetworkContractRepository = new OutofNetworkContractRepository(_context);
            ProviderByLocationRepository = new ProviderByLocationRepository(_context);
            UserRepository = new UserRepository(_context);
            RoleRepository = new RoleRepository(_context);
            SearchCredentialsRepository = new SearchCredentialsRepository(_context);
            PersonalFileRepository = new PersonalFileRepository(_context);
            PersonalFileTypeRepository = new PersonalFileTypeRepository(_context);
            UserCorporationAccessRepository = new UserCorporationAccessRepository(_context);
            ExpireDateNotificationRepository = new ExpireDateNotificationRepository(_context);
            ScheduleRepository = new ScheduleRepository(_context);
            DoctorScheduleRepository = new DoctorScheduleRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}