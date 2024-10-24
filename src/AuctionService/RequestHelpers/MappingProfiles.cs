using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using BuildingBlocks.Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(s => s.Item);
        CreateMap<Item, AuctionDto>();

        CreateMap<Auction, AuctionCreated>().IncludeMembers(s => s.Item);
        CreateMap<Item, AuctionCreated>();

        CreateMap<CreateAuctionDto, Auction>().ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
    }
}
