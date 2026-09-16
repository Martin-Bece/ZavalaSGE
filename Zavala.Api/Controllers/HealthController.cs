using Microsoft.AspNetCore.Mvc;

namespace Zavala.Api.Controllers;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]
[Authorize]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "ok", timestamp = DateTime.Now });
    }
}