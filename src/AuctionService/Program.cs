using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the Http request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    await app.InitDb();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

app.Run();
