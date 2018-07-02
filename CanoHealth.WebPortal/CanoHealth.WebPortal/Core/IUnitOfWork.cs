using CanoHealth.WebPortal.Core.Repositories;
using System;

namespace CanoHealth.WebPortal.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICorporationRepository Corporations { get; }

        IPlaceOfServiceRepository PlaceOfServices { get; }

        ILicenseRepository Licenses { get; }

        IAuditLogReporitory AuditLogs { get; }

        ILicenseTypeRepository LicenseTypes { get; }

        IInsuranceRepository Insurances { get; }

        ILineofBusinessRepository LineOfBusinesses { get; }

        IContractRepository Contracts { get; }

        IAddendumRepository Addendums { get; }

        IContractBusinessLineRepository ContracBusinessLineRepository { get; }

        IContractBusinessLineClinicRepository ContracBusinessLineClinicRepository { get; }

        IDoctorRepository Doctors { get; }

        IMedicalLicenseRepository MedicalLicenses { get; }

        IMedicalLicenseTypeRepository MedicalLicenseTypes { get; }

        IClinicDoctorRepository ClinicDoctor { get; }

        IDoctorLinkedContractRepository DoctorLinkedContracts { get; }

        ILinkedContractRepository LinkedContractStoredProcedures { get; }

        IInsuranceBusinessLineRepository InsuranceBusinessLineRepository { get; }

        IIndividualProviderRepository IndividualProviderRepository { get; }

        IOutofNetworkContractRepository OutofNetworkContractRepository { get; }

        IProviderByLocationRepository ProviderByLocationRepository { get; }

        IUserRepository UserRepository { get; }

        IRoleRepository RoleRepository { get; }

        ISearchCredentialsRepository SearchCredentialsRepository { get; }

        IPersonalFileRepository PersonalFileRepository { get; }

        IPersonalFileTypeRepository PersonalFileTypeRepository { get; }

        IUserCorporationAccessRepository UserCorporationAccessRepository { get; }

        IExpireDateNotificationRepository ExpireDateNotificationRepository { get; }

        int Complete();
    }
}
