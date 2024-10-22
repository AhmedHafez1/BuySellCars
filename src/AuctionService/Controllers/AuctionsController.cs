using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? dateString)
        {
            var query = _context.Auctions.Include(a => a.Item).AsQueryable();

            // Filter by date
            if (!string.IsNullOrEmpty(dateString))
            {
                if (DateTime.TryParse(dateString, out var date))
                {
                    // Convert the date to UTC if necessary
                    if (date.Kind == DateTimeKind.Local)
                    {
                        date = date.ToUniversalTime();
                    }
                    query = query.Where(x => x.UpdatedAt > date);
                }
                else
                {
                    return BadRequest("Date is not valid!");
                }
            }

            var auctions = await query.OrderBy(a => a.Item.Make).ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context
                .Auctions.Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction is null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            // TODO: add current user as a seller
            auction.Seller = "Ahmad";

            _context.Auctions.Add(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes");

            return CreatedAtAction(
                nameof(GetAuctionById),
                new { auction.Id },
                _mapper.Map<AuctionDto>(auction)
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
        {
            var auction = await _context
                .Auctions.Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);

            // TODO: check the current user is the seller
            if (auction is null)
                return BadRequest();

            auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            // TODO: check the current user is the seller

            if (auction is null)
                return BadRequest();

            _context.Auctions.Remove(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes");

            return Ok();
        }
    }
}
