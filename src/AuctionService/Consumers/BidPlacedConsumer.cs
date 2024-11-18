using AuctionService.Data;
using BuildingBlocks.Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dbContext;

    public BidPlacedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

        if (
            auction!.CurrentHighBid == null
            || (
                context.Message.Amount > auction.CurrentHighBid
                && context.Message.BidStatus.Contains("Accepted")
            )
        )
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _dbContext.SaveChangesAsync();
        }
    }
}
