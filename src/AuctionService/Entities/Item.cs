namespace AuctionService.Entities;

public class Item
{
    public Guid Id { get; set; }
    public string Make { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public string Color { get; set; } = default!;
    public int Mileage { get; set; }
    public string ImageUrl { get; set; } = default!;

    // Nav Props
    public Auction Auction { get; set; } = default!;
    public Guid AuctionId { get; set; }
}
