using AutoMapper;
using FBR_DI.Application.DTOs.Company;
using FBR_DI.Application.DTOs.Invoice;
using FBR_DI.Domain.Entities;

namespace FBR_DI.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Company mappings
        CreateMap<Company, CompanyListDto>()
            .ForMember(d => d.Environment, o => o.MapFrom(s => s.SubmissionEnvironment));

        CreateMap<Company, CompanyDetailDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();

        // Invoice mappings
        CreateMap<Invoice, InvoiceListDto>()
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.Items.Count));

        CreateMap<Invoice, InvoiceDetailDto>()
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.CompanyName : string.Empty));

        CreateMap<InvoiceItem, InvoiceItemDetailDto>();
    }
}
