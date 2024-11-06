using AutoMapper;
using BuildingBlocks.Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer(IMapper mapper) : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        var item = mapper.Map<Item>(context.Message);

        await DB.Update<Item>()
            .MatchID(item.ID)
            .ModifyOnly(
                x => new
                {
                    x.Make,
                    x.Model,
                    x.Year,
                    x.Color,
                    x.Mileage,
                },
                item
            )
            .ExecuteAsync();
    }
}
