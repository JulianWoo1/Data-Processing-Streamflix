using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandomFactController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RandomFactController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetRandomFact()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync("https://uselessfacts.jsph.pl/api/v2/facts/random");
                response.EnsureSuccessStatusCode();
                var fact = await response.Content.ReadAsStringAsync();
                return Content(fact, "application/json");
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Request failed: {e.Message}");
            }
        }
    }
}
