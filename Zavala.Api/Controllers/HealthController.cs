using Microsoft.AspNetCore.Mvc;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "ok", timestamp = DateTime.Now });
    }
}