using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        }
    );
});

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
