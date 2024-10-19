using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

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

        if (count == 0)
        {
            var auctionsData = await File.ReadAllTextAsync("Data/auctions.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var auctions = JsonSerializer.Deserialize<List<Item>>(auctionsData, options);

            await DB.SaveAsync(auctions!);
        }
    }
}
