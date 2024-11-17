using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using BuildingBlocks.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(
            AuctionDbContext context,
            IMapper mapper,
            IPublishEndpoint publishEndpoint
        )
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);

            auction.Seller = User.Identity!.Name!;

            _context.Auctions.Add(auction);

            // Send message to message broker
            var auctionCreated = _mapper.Map<AuctionCreated>(auction);
            await _publishEndpoint.Publish(auctionCreated);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes");

            return CreatedAtAction(
                nameof(GetAuctionById),
                new { auction.Id },
                _mapper.Map<AuctionDto>(auction)
            );
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
        {
            var auction = await _context
                .Auctions.Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction is null)
                return BadRequest();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

            // Send message to message broker
            var updatedAuction = _mapper.Map<AuctionUpdated>(auction);
            await _publishEndpoint.Publish(updatedAuction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes");

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction is null)
                return BadRequest();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            _context.Auctions.Remove(auction);

            // Send message to message broker
            var deletedAuction = new AuctionDeleted { Id = id.ToString() };
            await _publishEndpoint.Publish(deletedAuction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Couldn't save changes");

            return Ok();
        }
    }
}
