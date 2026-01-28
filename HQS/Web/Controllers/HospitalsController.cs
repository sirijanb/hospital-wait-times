using Microsoft.AspNetCore.Mvc;

namespace HQS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HospitalsController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Hospital Wait Times API is running");
    }
}
