using Application.Common.Responses;
using Application.Features.Customers.Commands.Create;
using Application.Features.Customers.Commands.Delete;
using Application.Features.Customers.Commands.Update;
using Application.Features.Customers.Queries.GetById;
using Application.Features.Customers.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Customers.Profiles;

public class CustomerMappingProfiles : Profile
{
    public CustomerMappingProfiles()
    {
        CreateMap<Customer, CreateCustomerCommand>().ReverseMap();
        CreateMap<Customer, CreatedCustomerResponse>().ReverseMap();

        CreateMap<Customer, UpdateCustomerCommand>().ReverseMap();
        CreateMap<Customer, UpdatedCustomerResponse>().ReverseMap();

        CreateMap<Customer, DeleteCustomerCommand>().ReverseMap();
        CreateMap<Customer, DeletedCustomerResponse>().ReverseMap();

        CreateMap<Customer, GetListCustomerListItemDto>().ReverseMap();
        CreateMap<Customer, GetByIdCustomerResponse>().ReverseMap();

        CreateMap<Paginate<Customer>, GetListResponse<GetListCustomerListItemDto>>().ReverseMap();
    }
}
