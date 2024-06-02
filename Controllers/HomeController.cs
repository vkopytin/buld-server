using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class HomeController : ControllerBase
{
  [Authorize("read:files")]
  [HttpGet]
  [ActionName("data")]
  public IActionResult Data()
  {
    return Ok(new { test = "test" });
  }
}