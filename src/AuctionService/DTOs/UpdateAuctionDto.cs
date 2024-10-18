namespace AuctionService.DTOs;

public class UpdateAuctionDto
{
    public string Make { get; set; } = default!;

    public string Model { get; set; } = default!;

    public int Year { get; set; }

    public string Color { get; set; } = default!;

    public int Mileage { get; set; }
}
