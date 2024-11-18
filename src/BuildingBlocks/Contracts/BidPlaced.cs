namespace BuildingBlocks.Contracts;

public class BidPlaced
{
    public string Id { get; set; } = default!;
    public string AuctionId { get; set; } = default!;
    public string Bidder { get; set; } = default!;
    public DateTime BidTime { get; set; }
    public int Amount { get; set; } = default!;
    public string BidStatus { get; set; } = default!;
}
