using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class HomeController : ControllerBase
{
  [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("user-login")]
  public IActionResult UserLogin()
  {
    return Ok(new { test = "test" });
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("data")]
  public IActionResult Data()
  {
    return Ok(new { test = "test" });
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

  [HttpGet]
  [ActionName("public")]
  public IActionResult Public()
  {
    return Ok(new { Test = "test" });
  }
}