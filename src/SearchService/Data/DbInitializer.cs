using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public static class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        // Init Db Connection
        await DB.InitAsync(
            "SearchDb",
            MongoClientSettings.FromConnectionString(
                app.Configuration.GetConnectionString("Default")
            )
        );

        // Create indexes for the fields used in search
        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        // Seed data
        var count = await DB.CountAsync<Item>();

        var scope = app.Services.CreateScope();
        var http = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await http.GetItemsForSearchDb();

        if (items?.Count > 0)
        {
            Console.WriteLine(items.Count + " returned from the auction service");
            await DB.SaveAsync(items);
        }
    }
}
