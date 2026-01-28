using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/geo")]
public class GeoController : ControllerBase
{
    private readonly HttpClient _http;

    public GeoController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient();
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "HQS/1.0 (contact@hqs.ca)"
        );
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAddress(
        [FromQuery] string q)
    {
        var url =
            $"https://nominatim.openstreetmap.org/search" +
            $"?q={Uri.EscapeDataString(q)}" +
            $"&countrycodes=ca" +
            $"&format=json&limit=5";

        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        return Content(json, "application/json");
    }
}
