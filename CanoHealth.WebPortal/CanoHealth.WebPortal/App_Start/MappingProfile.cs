using AutoMapper;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.ViewModels;
using CanoHealth.WebPortal.ViewModels.Admin;

namespace CanoHealth.WebPortal.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Corporation, CorporationViewModel>();
            Mapper.CreateMap<CorporationFormViewModel, Corporation>();
            Mapper.CreateMap<Corporation, CorporationDto>();

            Mapper.CreateMap<PlaceOfService, PlaceOfServiceFormViewModel>();
            Mapper.CreateMap<PlaceOfServiceFormViewModel, PlaceOfService>();
            Mapper.CreateMap<PlaceOfService, PlaceOfServiceDto>();

            Mapper.CreateMap<LicenseType, LicenseTypeDto>();

            Mapper.CreateMap<Insurance, InsuranceDto>();
            Mapper.CreateMap<InsuranceFormDto, Insurance>();
            Mapper.CreateMap<Insurance, InsuranceFormViewModel>();
            Mapper.CreateMap<InsuranceFormViewModel, Insurance>();

            Mapper.CreateMap<PlanType, LineofBusinessDto>();
            Mapper.CreateMap<PlanType, BusinessLineViewModel>();
            Mapper.CreateMap<BusinessLineViewModel,PlanType>();

            Mapper.CreateMap<ContractAddendum, ContractAddendumDto>();
            Mapper.CreateMap<ContractAddendumFormViewModel, ContractAddendum>();

            Mapper.CreateMap<Doctor, DoctorFormViewModel>();
            Mapper.CreateMap<DoctorFormViewModel, Doctor>();
            Mapper.CreateMap<Doctor, DoctorDto>();
            Mapper.CreateMap<DoctorDto, Doctor>();

            Mapper.CreateMap<DoctorFile, DoctorPersonalFileDto>()
                .ForMember(t => t.DoctorFileTypeName,
                    opt => opt.MapFrom(d => d.DoctorFileType.DoctorFileTypeName));

            Mapper.CreateMap<DoctorFile, DoctorFileFormViewModel>();
            Mapper.CreateMap<DoctorFileFormViewModel, DoctorFile>();

            Mapper.CreateMap<DoctorFileType, DoctorFileTypeDto>();

            Mapper.CreateMap<MedicalLicenseType, MedicalLicenseTypeDto>();

            Mapper.CreateMap<ProviderByLocation, ProviderByLocationDto>();
            Mapper.CreateMap<ProviderByLocationDto, ProviderByLocation>();

            Mapper.CreateMap<ApplicationRole, RoleViewModel>();
        }
    }
}