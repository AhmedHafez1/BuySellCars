using AutoMapper;
using BuildingBlocks.Contracts;
using SearchService.Models;

namespace SearchService.RequestHelper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}
