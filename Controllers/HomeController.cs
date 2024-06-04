using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

  [Authorize]
  [HttpGet]
  [ActionName("index")]
  public IActionResult Index()
  {
    var claims = new Dictionary<string, Dictionary<string, string>>();

    foreach (var claim in User.Claims)
    {
      claims.Add(claim.Type, new Dictionary<string, string>()
      {
        ["value"] = claim.Value,
        ["Issuer"] = claim.Issuer,
      });
    }

    return Ok(claims);
  }
}