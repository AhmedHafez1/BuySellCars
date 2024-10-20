using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionServiceHttpClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public AuctionServiceHttpClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<List<Item>?> GetItemsForSearchDb()
    {
        var lastUpdatedDate = await DB.Find<Item, string>()
            .Sort(x => x.Descending(a => a.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        return await _http.GetFromJsonAsync<List<Item>>(
            $"{_config["AuctionServiceUrl"]}/api/auctions?dateString={lastUpdatedDate}"
        );
    }
}
