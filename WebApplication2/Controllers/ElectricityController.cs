using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ElectricityController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }


        [HttpGet("GetSahko")]
        public async Task<IActionResult> GetLatestPrices()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.Constants.PorssisahkoUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                await SendDataToOtherService(content, "https://localhost:7250/api/Prices");

                return Ok(content);
            }
            catch (HttpRequestException e)
            {
                // Log error (e.Message) or handle accordingly
                return StatusCode(500, "Virhe datan haussa: " + e.Message);
            }
        }


        private async Task SendDataToOtherService(string content, string url)
        {
            //var url = "http://localhost:7250/api/Prices/AddSpotPrice"; // Update with the actual URL of MicroserviceDB endpoint
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, stringContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Server response: {responseContent}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error sending request: {e.Message}");
            }
        }
    }
}
