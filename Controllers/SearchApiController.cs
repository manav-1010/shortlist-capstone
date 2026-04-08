using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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
            if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
            {
                return BadRequest(new { error = "Invalid coordinates." });
            }

            radiusKm = Math.Clamp(radiusKm, 1, 25);
            var radiusM = radiusKm * 1000;

            var query = $@"
[out:json][timeout:25];
(
  node(around:{radiusM},{lat},{lng})[shop=supermarket];
  node(around:{radiusM},{lat},{lng})[shop=convenience];
  node(around:{radiusM},{lat},{lng})[shop=grocery];

  node(around:{radiusM},{lat},{lng})[amenity=bus_station];
  node(around:{radiusM},{lat},{lng})[highway=bus_stop];

  node(around:{radiusM},{lat},{lng})[leisure=fitness_centre];
  node(around:{radiusM},{lat},{lng})[amenity=gym];
  node(around:{radiusM},{lat},{lng})[leisure=sports_centre];

  node(around:{radiusM},{lat},{lng})[leisure=park];
  node(around:{radiusM},{lat},{lng})[leisure=pitch];

  node(around:{radiusM},{lat},{lng})[amenity=parking];

  node(around:{radiusM},{lat},{lng})[amenity=laundry];
  node(around:{radiusM},{lat},{lng})[shop=laundry];

  node(around:{radiusM},{lat},{lng})[amenity=police];
  node(around:{radiusM},{lat},{lng})[amenity=fire_station];
  node(around:{radiusM},{lat},{lng})[amenity=library];
  node(around:{radiusM},{lat},{lng})[amenity=community_centre];
);
out body;";

            var endpoints = new[]
            {
                "https://overpass-api.de/api/interpreter",
                "https://lz4.overpass-api.de/api/interpreter",
                "https://overpass.kumi.systems/api/interpreter"
            };

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(30);

                    client.DefaultRequestHeaders.UserAgent.Clear();
                    client.DefaultRequestHeaders.UserAgent.Add(
                        new ProductInfoHeaderValue("ShortlistApp", "1.0"));

                    using var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("data", query)
                    });


                    var resp = await client.PostAsync(endpoint, content);

                    if (!resp.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var json = await resp.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        continue;
                    }

                    return Content(json, "application/json");
                }
                catch
                {
                    continue;
                }
            }

            return Content("{\"elements\":[]}", "application/json");
        }
    }
}