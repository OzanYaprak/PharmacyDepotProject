using Application.Common.Responses;
using Application.Features.Suppliers.Commands.Create;
using Application.Features.Suppliers.Commands.Delete;
using Application.Features.Suppliers.Commands.Update;
using Application.Features.Suppliers.Queries.GetById;
using Application.Features.Suppliers.Queries.GetList;
using Application.Features.Suppliers.Queries.GetListByDynamic;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Suppliers.Profiles;

public class SupplierMappingProfiles : Profile
{
    public SupplierMappingProfiles()
    {
        CreateMap<Supplier, CreateSupplierCommand>().ReverseMap();
        CreateMap<Supplier, CreatedSupplierResponse>().ReverseMap();

        CreateMap<Supplier, UpdateSupplierCommand>().ReverseMap();
        CreateMap<Supplier, UpdatedSupplierResponse>().ReverseMap();

        CreateMap<Supplier, DeleteSupplierCommand>().ReverseMap();
        CreateMap<Supplier, DeletedSupplierResponse>().ReverseMap();

        CreateMap<Supplier, GetListSupplierListItemDto>().ReverseMap();
        CreateMap<Supplier, GetByIdSupplierResponse>().ReverseMap();

        CreateMap<Supplier, GetListByDynamicSupplierListItemDto>().ReverseMap();

        CreateMap<Paginate<Supplier>, GetListResponse<GetListSupplierListItemDto>>().ReverseMap();
        CreateMap<Paginate<Supplier>, GetListResponse<GetListByDynamicSupplierListItemDto>>().ReverseMap();
    }
}
