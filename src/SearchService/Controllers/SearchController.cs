using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm = "")
        {
            var query = DB.Find<Item>().Sort(x => x.Ascending(a => a.Make));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query.Match(Search.Full, searchTerm).SortByTextScore();
            }

            var items = await query.ExecuteAsync();

            return items;
        }
    }
}
