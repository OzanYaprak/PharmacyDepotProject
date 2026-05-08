using Application.Common.Responses;
using Application.Features.Warehouses.Commands.Create;
using Application.Features.Warehouses.Commands.Delete;
using Application.Features.Warehouses.Commands.Update;
using Application.Features.Warehouses.Queries.GetById;
using Application.Features.Warehouses.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Warehouses.Profiles;

public class WarehouseMappingProfiles : Profile
{
    public WarehouseMappingProfiles()
    {
        CreateMap<Warehouse, CreateWarehouseCommand>().ReverseMap();
        CreateMap<Warehouse, CreatedWarehouseResponse>().ReverseMap();

        CreateMap<Warehouse, UpdateWarehouseCommand>().ReverseMap();
        CreateMap<Warehouse, UpdatedWarehouseResponse>().ReverseMap();

        CreateMap<Warehouse, DeleteWarehouseCommand>().ReverseMap();
        CreateMap<Warehouse, DeletedWarehouseResponse>().ReverseMap();

        CreateMap<Warehouse, GetListWarehouseListItemDto>().ReverseMap();
        CreateMap<Warehouse, GetByIdWarehouseResponse>().ReverseMap();

        CreateMap<Paginate<Warehouse>, GetListResponse<GetListWarehouseListItemDto>>().ReverseMap();
    }
}
