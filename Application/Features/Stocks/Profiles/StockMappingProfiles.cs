using Application.Common.Responses;
using Application.Features.Stocks.Commands.Create;
using Application.Features.Stocks.Commands.Delete;
using Application.Features.Stocks.Commands.Update;
using Application.Features.Stocks.Queries.GetById;
using Application.Features.Stocks.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using Persistence.Paging;

namespace Application.Features.Stocks.Profiles;

public class StockMappingProfiles : Profile
{
    public StockMappingProfiles()
    {
        CreateMap<Stock, CreateStockCommand>().ReverseMap();
        CreateMap<Stock, CreatedStockResponse>().ReverseMap();

        CreateMap<Stock, UpdateStockCommand>().ReverseMap();
        CreateMap<Stock, UpdatedStockResponse>().ReverseMap();

        CreateMap<Stock, DeleteStockCommand>().ReverseMap();
        CreateMap<Stock, DeletedStockResponse>().ReverseMap();

        CreateMap<Stock, GetListStockListItemDto>().ReverseMap();
        CreateMap<Stock, GetByIdStockResponse>().ReverseMap();

        CreateMap<Paginate<Stock>, GetListResponse<GetListStockListItemDto>>().ReverseMap();
    }
}
