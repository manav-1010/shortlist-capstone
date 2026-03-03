using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Shortlist.Web.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SearchApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] int radiusKm = 3)
        {
            radiusKm = Math.Clamp(radiusKm, 1, 25);
            var radiusM = radiusKm * 1000;

            // POIs that map nicely to your priorities
            var query = $@"
[out:json][timeout:25];
(
  node(around:{radiusM},{lat},{lng})[shop=supermarket];
  node(around:{radiusM},{lat},{lng})[amenity=bus_station];
  node(around:{radiusM},{lat},{lng})[highway=bus_stop];
  node(around:{radiusM},{lat},{lng})[leisure=fitness_centre];
  node(around:{radiusM},{lat},{lng})[leisure=park];
  node(around:{radiusM},{lat},{lng})[amenity=parking];
  node(around:{radiusM},{lat},{lng})[amenity=laundry];
  node(around:{radiusM},{lat},{lng})[shop=laundry];
);
out;";

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            using var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", query)
            });

            var resp = await client.PostAsync("https://overpass-api.de/api/interpreter", content);

            if (!resp.IsSuccessStatusCode)
            {
                return StatusCode((int)resp.StatusCode, new { error = "Overpass API request failed." });
            }

            var json = await resp.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}